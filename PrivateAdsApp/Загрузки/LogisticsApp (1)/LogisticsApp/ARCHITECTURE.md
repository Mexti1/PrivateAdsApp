# Архитектура приложения Logistics App

## Обзор

Приложение построено на архитектуре MVVM (Model-View-ViewModel) с использованием современных технологий Android.

## Слои приложения

### 1. Presentation Layer (UI)

**Технологии:**
- Jetpack Compose - для создания UI
- Navigation Compose - для навигации
- Material 3 Design - дизайн система

**Компоненты:**
- `MainActivity.kt` - точка входа в приложение
- `MainScreen.kt` - главный экран с навигацией
- Screens - отдельные экраны приложения:
  - `DashboardScreen` - главная страница со статистикой
  - `OrdersScreen` - список заказов
  - `OrderDetailScreen` - детали заказа
  - `CreateOrderScreen` - создание заказа
  - `TrackingScreen` - отслеживание груза

### 2. ViewModel Layer

**Технологии:**
- ViewModel из Android Architecture Components
- StateFlow для реактивного управления состоянием
- Kotlin Coroutines для асинхронности

**ViewModels:**
- `OrdersViewModel` - управление списком заказов
- `CreateOrderViewModel` - создание заказа
- `TrackingViewModel` - отслеживание
- `DashboardViewModel` - статистика

**Паттерны:**
- Single Source of Truth
- Unidirectional Data Flow
- Reactive Programming

### 3. Domain Layer

**Модели данных:**
- `Order` - модель заказа
- `OrderStatus` - статусы заказа
- `Driver` - модель водителя
- `Vehicle` - модель транспорта
- `Client` - модель клиента
- `TrackingInfo` - информация для отслеживания
- `Location` - модель местоположения

### 4. Data Layer

**Технологии:**
- Repository Pattern
- Kotlin Coroutines

**Репозитории:**
- `OrderRepository` - работа с заказами

**Будущие интеграции:**
- Room Database - локальное хранилище
- Retrofit - сетевые запросы
- DataStore - настройки приложения

## Поток данных

```
User Action → View → ViewModel → Repository → Data Source
                ↑        ↓
                ←  State  ←
```

1. Пользователь взаимодействует с UI (View)
2. View вызывает методы ViewModel
3. ViewModel использует Repository для получения/изменения данных
4. Repository работает с источниками данных (API, БД)
5. Данные возвращаются через StateFlow
6. View реагирует на изменения State и обновляется

## Управление состоянием

### StateFlow Pattern

```kotlin
// ViewModel
private val _uiState = MutableStateFlow<UiState>(UiState.Loading)
val uiState: StateFlow<UiState> = _uiState.asStateFlow()

// View (Compose)
val uiState by viewModel.uiState.collectAsState()
```

### UiState Types

```kotlin
sealed class UiState {
    object Loading : UiState()
    data class Success<T>(val data: T) : UiState()
    data class Error(val message: String) : UiState()
}
```

## Навигация

### Navigation Graph

```
MainScreen (NavHost)
├── Dashboard (startDestination)
├── Orders
│   ├── OrderDetail
│   └── CreateOrder
└── Tracking
```

### Navigation Flow

```kotlin
NavHost(navController, startDestination = "dashboard") {
    composable("dashboard") { DashboardScreen() }
    composable("orders") { OrdersScreen() }
    composable("order_detail") { OrderDetailScreen() }
    composable("create_order") { CreateOrderScreen() }
    composable("tracking") { TrackingScreen() }
}
```

## Dependency Injection

**Текущая реализация:** Manual DI (создание в конструкторах)

**Рекомендуемые решения для масштабирования:**
- Hilt (рекомендуется для Android)
- Koin (легковесный)

### Пример с Hilt:

```kotlin
@HiltViewModel
class OrdersViewModel @Inject constructor(
    private val repository: OrderRepository
) : ViewModel() {
    // ...
}
```

## Обработка ошибок

### Централизованная обработка

```kotlin
sealed class Result<out T> {
    data class Success<T>(val data: T) : Result<T>()
    data class Error(val exception: Exception) : Result<Nothing>()
}
```

### В Repository:

```kotlin
suspend fun getOrders(): Result<List<Order>> {
    return try {
        val orders = api.getOrders()
        Result.Success(orders)
    } catch (e: Exception) {
        Result.Error(e)
    }
}
```

### В ViewModel:

```kotlin
viewModelScope.launch {
    _uiState.value = UiState.Loading
    when (val result = repository.getOrders()) {
        is Result.Success -> _uiState.value = UiState.Success(result.data)
        is Result.Error -> _uiState.value = UiState.Error(result.exception.message)
    }
}
```

## Lifecycle Management

### ViewModel Lifecycle

- ViewModel переживает поворот экрана
- Очищается при уничтожении Activity/Fragment
- Использует `viewModelScope` для корутин

### Compose Lifecycle

```kotlin
LaunchedEffect(key) {
    // Выполняется при изменении key
}

DisposableEffect(key) {
    // Настройка
    onDispose {
        // Очистка
    }
}
```

## Тестирование

### Unit Tests

```kotlin
class OrdersViewModelTest {
    @Test
    fun `load orders success`() = runTest {
        // Given
        val repository = FakeOrderRepository()
        val viewModel = OrdersViewModel(repository)
        
        // When
        viewModel.loadOrders()
        
        // Then
        val state = viewModel.uiState.value
        assertTrue(state is UiState.Success)
    }
}
```

### UI Tests (Compose)

```kotlin
@Test
fun ordersList_displays_correctly() {
    composeTestRule.setContent {
        OrdersScreen()
    }
    
    composeTestRule
        .onNodeWithText("ORD-2025-001")
        .assertIsDisplayed()
}
```

## Performance Optimization

### 1. Compose Optimization

```kotlin
// Использование remember для избежания рекомпозиции
val state = remember { mutableStateOf(value) }

// Использование derivedStateOf для производных значений
val filteredOrders = remember(orders, filter) {
    derivedStateOf { orders.filter { it.status == filter } }
}
```

### 2. LazyColumn Optimization

```kotlin
LazyColumn {
    items(
        items = orders,
        key = { it.id } // Важно для производительности
    ) { order ->
        OrderCard(order)
    }
}
```

### 3. Coroutines

```kotlin
// Использование диспетчеров
viewModelScope.launch(Dispatchers.IO) {
    // Сетевые/БД операции
    val data = repository.getData()
    
    withContext(Dispatchers.Main) {
        // Обновление UI
        _uiState.value = UiState.Success(data)
    }
}
```

## Security

### 1. Хранение токенов

```kotlin
// Использование EncryptedSharedPreferences
val masterKey = MasterKey.Builder(context)
    .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
    .build()

val sharedPreferences = EncryptedSharedPreferences.create(
    context,
    "secure_prefs",
    masterKey,
    EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
    EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
)
```

### 2. Network Security

```xml
<!-- network_security_config.xml -->
<network-security-config>
    <domain-config cleartextTrafficPermitted="false">
        <domain includeSubdomains="true">api.logistics-app.com</domain>
    </domain-config>
</network-security-config>
```

### 3. Certificate Pinning

```kotlin
val certificatePinner = CertificatePinner.Builder()
    .add("api.logistics-app.com", "sha256/AAAAAAAAAA...")
    .build()

val client = OkHttpClient.Builder()
    .certificatePinner(certificatePinner)
    .build()
```

## Локализация

### Поддержка языков

```
res/
├── values/              # Английский (по умолчанию)
│   └── strings.xml
├── values-ru/           # Русский
│   └── strings.xml
└── values-en/           # Английский (явно)
    └── strings.xml
```

## Offline Support

### Room Database

```kotlin
@Entity(tableName = "orders")
data class OrderEntity(
    @PrimaryKey val id: String,
    val orderNumber: String,
    // ...
)

@Dao
interface OrderDao {
    @Query("SELECT * FROM orders")
    fun getAllOrders(): Flow<List<OrderEntity>>
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertOrders(orders: List<OrderEntity>)
}
```

### Sync Strategy

```kotlin
class SyncRepository {
    suspend fun syncOrders() {
        try {
            val remoteOrders = api.getOrders()
            database.orderDao().insertOrders(remoteOrders)
        } catch (e: Exception) {
            // Handle offline mode
        }
    }
}
```

## Monitoring & Analytics

### Crashlytics

```kotlin
FirebaseCrashlytics.getInstance().apply {
    setUserId(userId)
    log("Order created: $orderId")
}
```

### Analytics

```kotlin
FirebaseAnalytics.getInstance(context).logEvent("order_created") {
    param("order_id", orderId)
    param("price", price)
}
```

## CI/CD Pipeline

### Рекомендуемый workflow:

1. **Build**
   - Gradle build
   - Lint checks
   - Unit tests

2. **Test**
   - Unit tests
   - Integration tests
   - UI tests

3. **Deploy**
   - Build release APK
   - Sign APK
   - Upload to Play Store

### GitHub Actions Example:

```yaml
name: Android CI

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Set up JDK
        uses: actions/setup-java@v2
      - name: Build
        run: ./gradlew build
      - name: Test
        run: ./gradlew test
```

## Future Improvements

1. **Модульная архитектура** - разделение на feature-модули
2. **Clean Architecture** - добавление UseCase слоя
3. **Reactive Streams** - миграция на Flow/RxJava
4. **Offline-First** - полная поддержка офлайн режима
5. **Multi-module** - разделение на модули по функциональности
