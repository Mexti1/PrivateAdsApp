package com.logistics.app.ui.orders

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import com.logistics.app.data.model.Order
import com.logistics.app.data.model.OrderStatus
import java.text.NumberFormat
import java.text.SimpleDateFormat
import java.util.*

/**
 * Экран детальной информации о заказе
 */
@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun OrderDetailScreen(
    order: Order,
    onBackClick: () -> Unit = {},
    onUpdateStatus: (OrderStatus) -> Unit = {}
) {
    val dateFormat = SimpleDateFormat("dd.MM.yyyy HH:mm", Locale.getDefault())
    val currencyFormat = NumberFormat.getCurrencyInstance(Locale("ru", "RU"))
    var showStatusDialog by remember { mutableStateOf(false) }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(order.orderNumber) },
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
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
                .verticalScroll(rememberScrollState())
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            // Статус заказа
            Card(
                modifier = Modifier.fillMaxWidth(),
                colors = CardDefaults.cardColors(
                    containerColor = Color(order.status.getColor()).copy(alpha = 0.1f)
                )
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(16.dp),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Column {
                        Text(
                            text = "Статус",
                            style = MaterialTheme.typography.bodySmall,
                            color = Color.Gray
                        )
                        Text(
                            text = order.status.getDisplayName(),
                            style = MaterialTheme.typography.titleLarge,
                            fontWeight = FontWeight.Bold,
                            color = Color(order.status.getColor())
                        )
                    }
                    Button(
                        onClick = { showStatusDialog = true },
                        colors = ButtonDefaults.buttonColors(
                            containerColor = Color(order.status.getColor())
                        )
                    ) {
                        Text("Изменить")
                    }
                }
            }

            // Информация о клиенте
            InfoSection(
                title = "Информация о клиенте",
                icon = Icons.Default.Person
            ) {
                InfoRow("Имя", order.clientName)
                InfoRow("Телефон", order.clientPhone)
            }

            // Маршрут
            InfoSection(
                title = "Маршрут доставки",
                icon = Icons.Default.Place
            ) {
                Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    LocationInfo("Откуда", order.pickupAddress, isStart = true)
                    LocationInfo("Куда", order.deliveryAddress, isStart = false)
                }
            }

            // Информация о грузе
            InfoSection(
                title = "Информация о грузе",
                icon = Icons.Default.ShoppingCart
            ) {
                InfoRow("Описание", order.cargoDescription)
                InfoRow("Вес", "${order.weight} кг")
                InfoRow("Объем", "${order.volume} м³")
            }

            // Информация о водителе
            if (order.driverName != null) {
                InfoSection(
                    title = "Водитель и транспорт",
                    icon = Icons.Default.Build
                ) {
                    InfoRow("Водитель", order.driverName)
                    InfoRow("Транспортное средство", order.vehicleNumber ?: "Не указано")
                }
            }

            // Даты и время
            InfoSection(
                title = "Даты",
                icon = Icons.Default.DateRange
            ) {
                InfoRow("Дата создания", dateFormat.format(order.createdDate))
                if (order.estimatedDeliveryDate != null) {
                    InfoRow(
                        "Планируемая доставка",
                        dateFormat.format(order.estimatedDeliveryDate)
                    )
                }
                if (order.actualDeliveryDate != null) {
                    InfoRow(
                        "Фактическая доставка",
                        dateFormat.format(order.actualDeliveryDate)
                    )
                }
            }

            // Стоимость
            Card(
                modifier = Modifier.fillMaxWidth(),
                colors = CardDefaults.cardColors(
                    containerColor = MaterialTheme.colorScheme.primaryContainer
                )
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(16.dp),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = "Стоимость доставки",
                        style = MaterialTheme.typography.titleMedium,
                        fontWeight = FontWeight.Bold
                    )
                    Text(
                        text = currencyFormat.format(order.price),
                        style = MaterialTheme.typography.headlineMedium,
                        fontWeight = FontWeight.Bold,
                        color = MaterialTheme.colorScheme.primary
                    )
                }
            }

            // Трек-номер
            Card(
                modifier = Modifier.fillMaxWidth(),
                colors = CardDefaults.cardColors(
                    containerColor = MaterialTheme.colorScheme.surface
                )
            ) {
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(16.dp)
                ) {
                    Text(
                        text = "Номер отслеживания",
                        style = MaterialTheme.typography.bodySmall,
                        color = Color.Gray
                    )
                    Spacer(modifier = Modifier.height(4.dp))
                    Text(
                        text = order.trackingNumber,
                        style = MaterialTheme.typography.titleMedium,
                        fontWeight = FontWeight.Bold
                    )
                }
            }

            // Примечания
            if (!order.notes.isNullOrEmpty()) {
                InfoSection(
                    title = "Примечания",
                    icon = Icons.Default.Info
                ) {
                    Text(
                        text = order.notes,
                        style = MaterialTheme.typography.bodyMedium
                    )
                }
            }
        }
    }

    // Диалог изменения статуса
    if (showStatusDialog) {
        StatusUpdateDialog(
            currentStatus = order.status,
            onDismiss = { showStatusDialog = false },
            onConfirm = { newStatus ->
                onUpdateStatus(newStatus)
                showStatusDialog = false
            }
        )
    }
}

/**
 * Секция информации
 */
@Composable
fun InfoSection(
    title: String,
    icon: ImageVector,
    content: @Composable ColumnScope.() -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surface
        )
    ) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp)
        ) {
            Row(
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                Icon(
                    icon,
                    contentDescription = null,
                    tint = MaterialTheme.colorScheme.primary
                )
                Text(
                    text = title,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold
                )
            }
            Spacer(modifier = Modifier.height(12.dp))
            content()
        }
    }
}

/**
 * Строка информации
 */
@Composable
fun InfoRow(label: String, value: String) {
    Column(modifier = Modifier.padding(vertical = 4.dp)) {
        Text(
            text = label,
            style = MaterialTheme.typography.bodySmall,
            color = Color.Gray
        )
        Text(
            text = value,
            style = MaterialTheme.typography.bodyLarge
        )
    }
}

/**
 * Информация о локации
 */
@Composable
fun LocationInfo(label: String, address: String, isStart: Boolean) {
    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.spacedBy(12.dp)
    ) {
        Box(
            modifier = Modifier
                .size(40.dp)
                .background(
                    if (isStart) MaterialTheme.colorScheme.primary.copy(alpha = 0.2f)
                    else MaterialTheme.colorScheme.secondary.copy(alpha = 0.2f),
                    shape = RoundedCornerShape(20.dp)
                ),
            contentAlignment = Alignment.Center
        ) {
            Icon(
                if (isStart) Icons.Default.Place else Icons.Default.LocationOn,
                contentDescription = null,
                tint = if (isStart) MaterialTheme.colorScheme.primary
                else MaterialTheme.colorScheme.secondary
            )
        }
        Column(modifier = Modifier.weight(1f)) {
            Text(
                text = label,
                style = MaterialTheme.typography.bodySmall,
                color = Color.Gray
            )
            Text(
                text = address,
                style = MaterialTheme.typography.bodyMedium
            )
        }
    }
}

/**
 * Диалог обновления статуса
 */
@Composable
fun StatusUpdateDialog(
    currentStatus: OrderStatus,
    onDismiss: () -> Unit,
    onConfirm: (OrderStatus) -> Unit
) {
    var selectedStatus by remember { mutableStateOf(currentStatus) }

    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("Изменить статус заказа") },
        text = {
            Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                OrderStatus.values().forEach { status ->
                    Row(
                        modifier = Modifier
                            .fillMaxWidth()
                            .background(
                                if (selectedStatus == status)
                                    Color(status.getColor()).copy(alpha = 0.2f)
                                else Color.Transparent,
                                shape = RoundedCornerShape(8.dp)
                            )
                            .padding(12.dp),
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        RadioButton(
                            selected = selectedStatus == status,
                            onClick = { selectedStatus = status }
                        )
                        Spacer(modifier = Modifier.width(8.dp))
                        Text(
                            text = status.getDisplayName(),
                            style = MaterialTheme.typography.bodyMedium
                        )
                    }
                }
            }
        },
        confirmButton = {
            Button(onClick = { onConfirm(selectedStatus) }) {
                Text("Применить")
            }
        },
        dismissButton = {
            TextButton(onClick = onDismiss) {
                Text("Отмена")
            }
        }
    )
}
