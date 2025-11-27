package com.logistics.app.ui.orders

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import androidx.lifecycle.viewmodel.compose.viewModel
import com.logistics.app.data.model.Order
import com.logistics.app.data.model.OrderStatus
import com.logistics.app.data.repository.OrderRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import java.util.*

/**
 * ViewModel для создания заказа
 */
class CreateOrderViewModel(
    private val repository: OrderRepository = OrderRepository()
) : ViewModel() {

    private val _uiState = MutableStateFlow<CreateOrderUiState>(CreateOrderUiState.Idle)
    val uiState: StateFlow<CreateOrderUiState> = _uiState.asStateFlow()

    fun createOrder(
        clientName: String,
        clientPhone: String,
        pickupAddress: String,
        deliveryAddress: String,
        cargoDescription: String,
        weight: Double,
        volume: Double,
        price: Double,
        estimatedDays: Int
    ) {
        viewModelScope.launch {
            _uiState.value = CreateOrderUiState.Loading
            try {
                val order = Order(
                    id = UUID.randomUUID().toString(),
                    orderNumber = "ORD-${System.currentTimeMillis()}",
                    clientName = clientName,
                    clientPhone = clientPhone,
                    pickupAddress = pickupAddress,
                    deliveryAddress = deliveryAddress,
                    cargoDescription = cargoDescription,
                    weight = weight,
                    volume = volume,
                    status = OrderStatus.PENDING,
                    createdDate = Date(),
                    estimatedDeliveryDate = Date(System.currentTimeMillis() + estimatedDays * 86400000L),
                    actualDeliveryDate = null,
                    price = price,
                    driverId = null,
                    driverName = null,
                    vehicleNumber = null,
                    trackingNumber = "TRK${System.currentTimeMillis()}"
                )
                
                val result = repository.createOrder(order)
                if (result.isSuccess) {
                    _uiState.value = CreateOrderUiState.Success(order)
                } else {
                    _uiState.value = CreateOrderUiState.Error(
                        result.exceptionOrNull()?.message ?: "Ошибка создания заказа"
                    )
                }
            } catch (e: Exception) {
                _uiState.value = CreateOrderUiState.Error(
                    e.message ?: "Ошибка создания заказа"
                )
            }
        }
    }

    fun resetState() {
        _uiState.value = CreateOrderUiState.Idle
    }
}

sealed class CreateOrderUiState {
    object Idle : CreateOrderUiState()
    object Loading : CreateOrderUiState()
    data class Success(val order: Order) : CreateOrderUiState()
    data class Error(val message: String) : CreateOrderUiState()
}

/**
 * Экран создания нового заказа
 */
@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun CreateOrderScreen(
    viewModel: CreateOrderViewModel = viewModel(),
    onBackClick: () -> Unit = {},
    onOrderCreated: (Order) -> Unit = {}
) {
    var clientName by remember { mutableStateOf("") }
    var clientPhone by remember { mutableStateOf("") }
    var pickupAddress by remember { mutableStateOf("") }
    var deliveryAddress by remember { mutableStateOf("") }
    var cargoDescription by remember { mutableStateOf("") }
    var weight by remember { mutableStateOf("") }
    var volume by remember { mutableStateOf("") }
    var price by remember { mutableStateOf("") }
    var estimatedDays by remember { mutableStateOf("") }

    val uiState by viewModel.uiState.collectAsState()

    LaunchedEffect(uiState) {
        if (uiState is CreateOrderUiState.Success) {
            onOrderCreated((uiState as CreateOrderUiState.Success).order)
            viewModel.resetState()
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Новый заказ") },
                navigationIcon = {
                    IconButton(onClick = onBackClick) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Назад")
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primary,
                    titleContentColor = Color.White,
                    navigationIconContentColor = Color.White
                )
            )
        }
    ) { paddingValues ->
        Box(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
        ) {
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .verticalScroll(rememberScrollState())
                    .padding(16.dp),
                verticalArrangement = Arrangement.spacedBy(16.dp)
            ) {
                // Информация о клиенте
                Text(
                    text = "Информация о клиенте",
                    style = MaterialTheme.typography.titleMedium,
                    color = MaterialTheme.colorScheme.primary
                )

                OutlinedTextField(
                    value = clientName,
                    onValueChange = { clientName = it },
                    label = { Text("Имя клиента") },
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true
                )

                OutlinedTextField(
                    value = clientPhone,
                    onValueChange = { clientPhone = it },
                    label = { Text("Телефон") },
                    modifier = Modifier.fillMaxWidth(),
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone),
                    singleLine = true
                )

                Divider(modifier = Modifier.padding(vertical = 8.dp))

                // Адреса
                Text(
                    text = "Маршрут доставки",
                    style = MaterialTheme.typography.titleMedium,
                    color = MaterialTheme.colorScheme.primary
                )

                OutlinedTextField(
                    value = pickupAddress,
                    onValueChange = { pickupAddress = it },
                    label = { Text("Адрес загрузки") },
                    modifier = Modifier.fillMaxWidth(),
                    minLines = 2
                )

                OutlinedTextField(
                    value = deliveryAddress,
                    onValueChange = { deliveryAddress = it },
                    label = { Text("Адрес доставки") },
                    modifier = Modifier.fillMaxWidth(),
                    minLines = 2
                )

                Divider(modifier = Modifier.padding(vertical = 8.dp))

                // Информация о грузе
                Text(
                    text = "Информация о грузе",
                    style = MaterialTheme.typography.titleMedium,
                    color = MaterialTheme.colorScheme.primary
                )

                OutlinedTextField(
                    value = cargoDescription,
                    onValueChange = { cargoDescription = it },
                    label = { Text("Описание груза") },
                    modifier = Modifier.fillMaxWidth(),
                    minLines = 2
                )

                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    OutlinedTextField(
                        value = weight,
                        onValueChange = { weight = it },
                        label = { Text("Вес (кг)") },
                        modifier = Modifier.weight(1f),
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                        singleLine = true
                    )

                    OutlinedTextField(
                        value = volume,
                        onValueChange = { volume = it },
                        label = { Text("Объем (м³)") },
                        modifier = Modifier.weight(1f),
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                        singleLine = true
                    )
                }

                Divider(modifier = Modifier.padding(vertical = 8.dp))

                // Стоимость и сроки
                Text(
                    text = "Стоимость и сроки",
                    style = MaterialTheme.typography.titleMedium,
                    color = MaterialTheme.colorScheme.primary
                )

                OutlinedTextField(
                    value = price,
                    onValueChange = { price = it },
                    label = { Text("Стоимость (₽)") },
                    modifier = Modifier.fillMaxWidth(),
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                    singleLine = true
                )

                OutlinedTextField(
                    value = estimatedDays,
                    onValueChange = { estimatedDays = it },
                    label = { Text("Срок доставки (дней)") },
                    modifier = Modifier.fillMaxWidth(),
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                    singleLine = true
                )

                Spacer(modifier = Modifier.height(16.dp))

                // Кнопка создания
                Button(
                    onClick = {
                        viewModel.createOrder(
                            clientName = clientName,
                            clientPhone = clientPhone,
                            pickupAddress = pickupAddress,
                            deliveryAddress = deliveryAddress,
                            cargoDescription = cargoDescription,
                            weight = weight.toDoubleOrNull() ?: 0.0,
                            volume = volume.toDoubleOrNull() ?: 0.0,
                            price = price.toDoubleOrNull() ?: 0.0,
                            estimatedDays = estimatedDays.toIntOrNull() ?: 1
                        )
                    },
                    modifier = Modifier.fillMaxWidth(),
                    enabled = uiState !is CreateOrderUiState.Loading &&
                            clientName.isNotBlank() &&
                            clientPhone.isNotBlank() &&
                            pickupAddress.isNotBlank() &&
                            deliveryAddress.isNotBlank() &&
                            cargoDescription.isNotBlank() &&
                            weight.isNotBlank() &&
                            volume.isNotBlank() &&
                            price.isNotBlank() &&
                            estimatedDays.isNotBlank()
                ) {
                    if (uiState is CreateOrderUiState.Loading) {
                        CircularProgressIndicator(
                            modifier = Modifier.size(24.dp),
                            color = Color.White
                        )
                    } else {
                        Text("Создать заказ")
                    }
                }

                if (uiState is CreateOrderUiState.Error) {
                    Text(
                        text = (uiState as CreateOrderUiState.Error).message,
                        color = MaterialTheme.colorScheme.error,
                        style = MaterialTheme.typography.bodyMedium
                    )
                }
            }
        }
    }
}
