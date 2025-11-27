package com.logistics.app.ui.tracking

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import androidx.lifecycle.viewmodel.compose.viewModel
import com.logistics.app.data.model.TrackingInfo
import com.logistics.app.data.repository.OrderRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import java.text.SimpleDateFormat
import java.util.*

/**
 * ViewModel для отслеживания
 */
class TrackingViewModel(
    private val repository: OrderRepository = OrderRepository()
) : ViewModel() {

    private val _uiState = MutableStateFlow<TrackingUiState>(TrackingUiState.Idle)
    val uiState: StateFlow<TrackingUiState> = _uiState.asStateFlow()

    fun trackOrder(trackingNumber: String) {
        viewModelScope.launch {
            _uiState.value = TrackingUiState.Loading
            try {
                val trackingInfo = repository.trackOrder(trackingNumber)
                if (trackingInfo != null) {
                    _uiState.value = TrackingUiState.Success(trackingInfo)
                } else {
                    _uiState.value = TrackingUiState.Error("Заказ с таким номером не найден")
                }
            } catch (e: Exception) {
                _uiState.value = TrackingUiState.Error(
                    e.message ?: "Ошибка поиска заказа"
                )
            }
        }
    }

    fun reset() {
        _uiState.value = TrackingUiState.Idle
    }
}

sealed class TrackingUiState {
    object Idle : TrackingUiState()
    object Loading : TrackingUiState()
    data class Success(val trackingInfo: TrackingInfo) : TrackingUiState()
    data class Error(val message: String) : TrackingUiState()
}

/**
 * Экран отслеживания груза
 */
@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun TrackingScreen(
    viewModel: TrackingViewModel = viewModel()
) {
    var trackingNumber by remember { mutableStateOf("") }
    val uiState by viewModel.uiState.collectAsState()

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Отслеживание груза") },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primary,
                    titleContentColor = Color.White
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
            // Поиск по трек-номеру
            Card(
                modifier = Modifier.fillMaxWidth(),
                colors = CardDefaults.cardColors(
                    containerColor = MaterialTheme.colorScheme.surface
                )
            ) {
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(16.dp),
                    verticalArrangement = Arrangement.spacedBy(12.dp)
                ) {
                    Text(
                        text = "Введите номер отслеживания",
                        style = MaterialTheme.typography.titleMedium,
                        fontWeight = FontWeight.Bold
                    )

                    OutlinedTextField(
                        value = trackingNumber,
                        onValueChange = { trackingNumber = it },
                        label = { Text("Номер отслеживания") },
                        placeholder = { Text("TRK2025001") },
                        modifier = Modifier.fillMaxWidth(),
                        singleLine = true,
                        leadingIcon = {
                            Icon(Icons.Default.Search, contentDescription = null)
                        }
                    )

                    Button(
                        onClick = { viewModel.trackOrder(trackingNumber) },
                        modifier = Modifier.fillMaxWidth(),
                        enabled = trackingNumber.isNotBlank() && uiState !is TrackingUiState.Loading
                    ) {
                        if (uiState is TrackingUiState.Loading) {
                            CircularProgressIndicator(
                                modifier = Modifier.size(24.dp),
                                color = Color.White
                            )
                        } else {
                            Text("Отследить")
                        }
                    }
                }
            }

            // Результат поиска
            when (uiState) {
                is TrackingUiState.Idle -> {
                    Card(
                        modifier = Modifier.fillMaxWidth(),
                        colors = CardDefaults.cardColors(
                            containerColor = MaterialTheme.colorScheme.surfaceVariant
                        )
                    ) {
                        Column(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(32.dp),
                            horizontalAlignment = Alignment.CenterHorizontally
                        ) {
                            Icon(
                                Icons.Default.Place,
                                contentDescription = null,
                                modifier = Modifier.size(64.dp),
                                tint = Color.Gray
                            )
                            Spacer(modifier = Modifier.height(16.dp))
                            Text(
                                text = "Введите номер отслеживания для поиска груза",
                                style = MaterialTheme.typography.bodyLarge,
                                color = Color.Gray
                            )
                        }
                    }
                }
                is TrackingUiState.Loading -> {
                    Box(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(32.dp),
                        contentAlignment = Alignment.Center
                    ) {
                        CircularProgressIndicator()
                    }
                }
                is TrackingUiState.Success -> {
                    val trackingInfo = (uiState as TrackingUiState.Success).trackingInfo
                    TrackingInfoCard(trackingInfo)
                }
                is TrackingUiState.Error -> {
                    Card(
                        modifier = Modifier.fillMaxWidth(),
                        colors = CardDefaults.cardColors(
                            containerColor = MaterialTheme.colorScheme.errorContainer
                        )
                    ) {
                        Row(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(16.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Icon(
                                Icons.Default.Warning,
                                contentDescription = null,
                                tint = MaterialTheme.colorScheme.error
                            )
                            Spacer(modifier = Modifier.width(12.dp))
                            Text(
                                text = (uiState as TrackingUiState.Error).message,
                                color = MaterialTheme.colorScheme.error
                            )
                        }
                    }
                }
            }
        }
    }
}

/**
 * Карточка с информацией об отслеживании
 */
@Composable
fun TrackingInfoCard(trackingInfo: TrackingInfo) {
    val dateFormat = SimpleDateFormat("dd.MM.yyyy HH:mm", Locale.getDefault())

    Column(verticalArrangement = Arrangement.spacedBy(16.dp)) {
        // Текущее местоположение
        Card(
            modifier = Modifier.fillMaxWidth(),
            colors = CardDefaults.cardColors(
                containerColor = MaterialTheme.colorScheme.primaryContainer
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
                        Icons.Default.LocationOn,
                        contentDescription = null,
                        tint = MaterialTheme.colorScheme.primary,
                        modifier = Modifier.size(32.dp)
                    )
                    Text(
                        text = "Текущее местоположение",
                        style = MaterialTheme.typography.titleMedium,
                        fontWeight = FontWeight.Bold
                    )
                }
                Spacer(modifier = Modifier.height(8.dp))
                Text(
                    text = trackingInfo.currentLocation.address,
                    style = MaterialTheme.typography.bodyLarge
                )
                Spacer(modifier = Modifier.height(4.dp))
                Text(
                    text = "Обновлено: ${dateFormat.format(trackingInfo.lastUpdate)}",
                    style = MaterialTheme.typography.bodySmall,
                    color = Color.Gray
                )
            }
        }

        // Планируемое прибытие
        if (trackingInfo.estimatedArrival != null) {
            Card(
                modifier = Modifier.fillMaxWidth(),
                colors = CardDefaults.cardColors(
                    containerColor = MaterialTheme.colorScheme.surface
                )
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(16.dp),
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Column {
                        Text(
                            text = "Ожидаемое прибытие",
                            style = MaterialTheme.typography.bodySmall,
                            color = Color.Gray
                        )
                        Text(
                            text = dateFormat.format(trackingInfo.estimatedArrival),
                            style = MaterialTheme.typography.titleMedium,
                            fontWeight = FontWeight.Bold
                        )
                    }
                    Icon(
                        Icons.Default.DateRange,
                        contentDescription = null,
                        tint = MaterialTheme.colorScheme.primary,
                        modifier = Modifier.size(32.dp)
                    )
                }
            }
        }

        // История статусов
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
                    text = "История перемещения",
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold
                )
                Spacer(modifier = Modifier.height(16.dp))

                trackingInfo.statusHistory.forEachIndexed { index, statusUpdate ->
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.spacedBy(12.dp)
                    ) {
                        // Линия времени
                        Column(
                            horizontalAlignment = Alignment.CenterHorizontally
                        ) {
                            Box(
                                modifier = Modifier
                                    .size(12.dp)
                                    .clip(CircleShape)
                                    .background(
                                        if (index == 0) MaterialTheme.colorScheme.primary
                                        else Color.Gray
                                    )
                            )
                            if (index < trackingInfo.statusHistory.size - 1) {
                                Box(
                                    modifier = Modifier
                                        .width(2.dp)
                                        .height(60.dp)
                                        .background(Color.Gray.copy(alpha = 0.3f))
                                )
                            }
                        }

                        // Информация о статусе
                        Column(
                            modifier = Modifier
                                .weight(1f)
                                .padding(bottom = 16.dp)
                        ) {
                            Text(
                                text = statusUpdate.status.getDisplayName(),
                                style = MaterialTheme.typography.bodyMedium,
                                fontWeight = FontWeight.Bold
                            )
                            Text(
                                text = dateFormat.format(statusUpdate.timestamp),
                                style = MaterialTheme.typography.bodySmall,
                                color = Color.Gray
                            )
                            if (statusUpdate.location != null) {
                                Text(
                                    text = statusUpdate.location.address,
                                    style = MaterialTheme.typography.bodySmall,
                                    color = Color.Gray
                                )
                            }
                            if (!statusUpdate.comment.isNullOrEmpty()) {
                                Text(
                                    text = statusUpdate.comment,
                                    style = MaterialTheme.typography.bodySmall,
                                    color = Color.Gray
                                )
                            }
                        }
                    }
                }
            }
        }
    }
}
