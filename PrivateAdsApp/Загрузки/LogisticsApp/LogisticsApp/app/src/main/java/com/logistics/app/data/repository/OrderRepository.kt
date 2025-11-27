package com.logistics.app.data.repository

import com.logistics.app.data.model.*
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.flow
import java.util.*

/**
 * Репозиторий для работы с заказами
 * В реальном приложении здесь будет работа с API и локальной БД
 */
class OrderRepository {

    // Мок-данные для демонстрации
    private val mockOrders = mutableListOf(
        Order(
            id = "1",
            orderNumber = "ORD-2025-001",
            clientName = "ООО \"Логистика Плюс\"",
            clientPhone = "+7 (999) 123-45-67",
            pickupAddress = "г. Москва, ул. Промышленная, д. 5",
            deliveryAddress = "г. Санкт-Петербург, пр. Невский, д. 120",
            cargoDescription = "Электронное оборудование",
            weight = 250.0,
            volume = 2.5,
            status = OrderStatus.IN_TRANSIT,
            createdDate = Date(System.currentTimeMillis() - 86400000 * 2),
            estimatedDeliveryDate = Date(System.currentTimeMillis() + 86400000),
            actualDeliveryDate = null,
            price = 25000.0,
            driverId = "D001",
            driverName = "Иванов Петр Сергеевич",
            vehicleNumber = "А123БВ777",
            trackingNumber = "TRK2025001"
        ),
        Order(
            id = "2",
            orderNumber = "ORD-2025-002",
            clientName = "ИП Смирнов А.В.",
            clientPhone = "+7 (999) 234-56-78",
            pickupAddress = "г. Москва, ул. Складская, д. 12",
            deliveryAddress = "г. Казань, ул. Баумана, д. 45",
            cargoDescription = "Мебель офисная",
            weight = 450.0,
            volume = 5.0,
            status = OrderStatus.CONFIRMED,
            createdDate = Date(System.currentTimeMillis() - 86400000),
            estimatedDeliveryDate = Date(System.currentTimeMillis() + 86400000 * 3),
            actualDeliveryDate = null,
            price = 35000.0,
            driverId = null,
            driverName = null,
            vehicleNumber = null,
            trackingNumber = "TRK2025002"
        ),
        Order(
            id = "3",
            orderNumber = "ORD-2025-003",
            clientName = "ООО \"Торговый Дом\"",
            clientPhone = "+7 (999) 345-67-89",
            pickupAddress = "г. Екатеринбург, ул. Заводская, д. 8",
            deliveryAddress = "г. Новосибирск, пр. Ленина, д. 234",
            cargoDescription = "Продукты питания",
            weight = 1200.0,
            volume = 8.0,
            status = OrderStatus.DELIVERED,
            createdDate = Date(System.currentTimeMillis() - 86400000 * 5),
            estimatedDeliveryDate = Date(System.currentTimeMillis() - 86400000 * 2),
            actualDeliveryDate = Date(System.currentTimeMillis() - 86400000 * 2),
            price = 52000.0,
            driverId = "D002",
            driverName = "Сидоров Алексей Иванович",
            vehicleNumber = "К456МН199",
            trackingNumber = "TRK2025003"
        ),
        Order(
            id = "4",
            orderNumber = "ORD-2025-004",
            clientName = "ЗАО \"Техснаб\"",
            clientPhone = "+7 (999) 456-78-90",
            pickupAddress = "г. Москва, ул. Промзона, д. 3",
            deliveryAddress = "г. Владивосток, ул. Портовая, д. 56",
            cargoDescription = "Запчасти автомобильные",
            weight = 800.0,
            volume = 6.5,
            status = OrderStatus.PENDING,
            createdDate = Date(System.currentTimeMillis()),
            estimatedDeliveryDate = Date(System.currentTimeMillis() + 86400000 * 7),
            actualDeliveryDate = null,
            price = 85000.0,
            driverId = null,
            driverName = null,
            vehicleNumber = null,
            trackingNumber = "TRK2025004"
        )
    )

    /**
     * Получить все заказы
     */
    suspend fun getAllOrders(): List<Order> {
        delay(500) // Имитация сетевого запроса
        return mockOrders.sortedByDescending { it.createdDate }
    }

    /**
     * Получить заказы по статусу
     */
    suspend fun getOrdersByStatus(status: OrderStatus): List<Order> {
        delay(500)
        return mockOrders.filter { it.status == status }
            .sortedByDescending { it.createdDate }
    }

    /**
     * Получить заказ по ID
     */
    suspend fun getOrderById(orderId: String): Order? {
        delay(300)
        return mockOrders.find { it.id == orderId }
    }

    /**
     * Создать новый заказ
     */
    suspend fun createOrder(order: Order): Result<Order> {
        delay(500)
        return try {
            mockOrders.add(0, order)
            Result.success(order)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    /**
     * Обновить статус заказа
     */
    suspend fun updateOrderStatus(orderId: String, newStatus: OrderStatus): Result<Order> {
        delay(500)
        return try {
            val index = mockOrders.indexOfFirst { it.id == orderId }
            if (index != -1) {
                val updatedOrder = mockOrders[index].copy(status = newStatus)
                mockOrders[index] = updatedOrder
                Result.success(updatedOrder)
            } else {
                Result.failure(Exception("Заказ не найден"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    /**
     * Назначить водителя на заказ
     */
    suspend fun assignDriver(orderId: String, driverId: String, driverName: String, vehicleNumber: String): Result<Order> {
        delay(500)
        return try {
            val index = mockOrders.indexOfFirst { it.id == orderId }
            if (index != -1) {
                val updatedOrder = mockOrders[index].copy(
                    driverId = driverId,
                    driverName = driverName,
                    vehicleNumber = vehicleNumber,
                    status = OrderStatus.ASSIGNED
                )
                mockOrders[index] = updatedOrder
                Result.success(updatedOrder)
            } else {
                Result.failure(Exception("Заказ не найден"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    /**
     * Отследить заказ
     */
    suspend fun trackOrder(trackingNumber: String): TrackingInfo? {
        delay(500)
        val order = mockOrders.find { it.trackingNumber == trackingNumber }
        return order?.let {
            TrackingInfo(
                orderId = it.id,
                currentLocation = Location(55.7558, 37.6173, "г. Москва, МКАД 50км"),
                lastUpdate = Date(),
                estimatedArrival = it.estimatedDeliveryDate,
                statusHistory = listOf(
                    StatusUpdate(
                        status = OrderStatus.PENDING,
                        timestamp = it.createdDate,
                        location = Location(55.7558, 37.6173, it.pickupAddress),
                        comment = "Заказ создан"
                    ),
                    StatusUpdate(
                        status = OrderStatus.CONFIRMED,
                        timestamp = Date(it.createdDate.time + 3600000),
                        location = Location(55.7558, 37.6173, it.pickupAddress),
                        comment = "Заказ подтвержден"
                    )
                )
            )
        }
    }

    /**
     * Получить статистику
     */
    suspend fun getStatistics(): OrderStatistics {
        delay(500)
        return OrderStatistics(
            totalOrders = mockOrders.size,
            pendingOrders = mockOrders.count { it.status == OrderStatus.PENDING },
            inTransitOrders = mockOrders.count { it.status == OrderStatus.IN_TRANSIT },
            deliveredOrders = mockOrders.count { it.status == OrderStatus.DELIVERED },
            cancelledOrders = mockOrders.count { it.status == OrderStatus.CANCELLED },
            totalRevenue = mockOrders.filter { it.status == OrderStatus.DELIVERED }
                .sumOf { it.price }
        )
    }
}

/**
 * Статистика по заказам
 */
data class OrderStatistics(
    val totalOrders: Int,
    val pendingOrders: Int,
    val inTransitOrders: Int,
    val deliveredOrders: Int,
    val cancelledOrders: Int,
    val totalRevenue: Double
)
