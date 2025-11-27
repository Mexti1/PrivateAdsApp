package com.logistics.app.ui.orders

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.logistics.app.data.model.Order
import com.logistics.app.data.model.OrderStatus
import com.logistics.app.data.repository.OrderRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

/**
 * ViewModel для экрана списка заказов
 */
class OrdersViewModel(
    private val repository: OrderRepository = OrderRepository()
) : ViewModel() {

    private val _uiState = MutableStateFlow<OrdersUiState>(OrdersUiState.Loading)
    val uiState: StateFlow<OrdersUiState> = _uiState.asStateFlow()

    private val _selectedFilter = MutableStateFlow<OrderStatus?>(null)
    val selectedFilter: StateFlow<OrderStatus?> = _selectedFilter.asStateFlow()

    init {
        loadOrders()
    }

    /**
     * Загрузить список заказов
     */
    fun loadOrders(filter: OrderStatus? = null) {
        viewModelScope.launch {
            _uiState.value = OrdersUiState.Loading
            try {
                val orders = if (filter != null) {
                    repository.getOrdersByStatus(filter)
                } else {
                    repository.getAllOrders()
                }
                _uiState.value = OrdersUiState.Success(orders)
            } catch (e: Exception) {
                _uiState.value = OrdersUiState.Error(
                    e.message ?: "Ошибка загрузки заказов"
                )
            }
        }
    }

    /**
     * Обновить фильтр
     */
    fun updateFilter(status: OrderStatus?) {
        _selectedFilter.value = status
        loadOrders(status)
    }

    /**
     * Обновить статус заказа
     */
    fun updateOrderStatus(orderId: String, newStatus: OrderStatus) {
        viewModelScope.launch {
            repository.updateOrderStatus(orderId, newStatus)
            loadOrders(_selectedFilter.value)
        }
    }

    /**
     * Обновить список
     */
    fun refresh() {
        loadOrders(_selectedFilter.value)
    }
}

/**
 * Состояния UI для экрана заказов
 */
sealed class OrdersUiState {
    object Loading : OrdersUiState()
    data class Success(val orders: List<Order>) : OrdersUiState()
    data class Error(val message: String) : OrdersUiState()
}
