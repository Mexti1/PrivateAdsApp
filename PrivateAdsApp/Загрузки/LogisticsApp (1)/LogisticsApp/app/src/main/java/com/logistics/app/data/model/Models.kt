package com.logistics.app.data.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize
import java.util.Date

/**
 * Модель заказа в транспортно-логистической системе
 */
@Parcelize
data class Order(
    val id: String,
    val orderNumber: String,
    val clientName: String,
    val clientPhone: String,
    val pickupAddress: String,
    val deliveryAddress: String,
    val cargoDescription: String,
    val weight: Double, // в кг
    val volume: Double, // в м³
    val status: OrderStatus,
    val createdDate: Date,
    val estimatedDeliveryDate: Date?,
    val actualDeliveryDate: Date?,
    val price: Double,
    val driverId: String?,
    val driverName: String?,
    val vehicleNumber: String?,
    val trackingNumber: String,
    val notes: String? = null
) : Parcelable

/**
 * Статусы заказа
 */
@Parcelize
enum class OrderStatus : Parcelable {
    PENDING,        // Ожидает обработки
    CONFIRMED,      // Подтвержден
    ASSIGNED,       // Назначен водитель
    IN_TRANSIT,     // В пути
    DELIVERED,      // Доставлен
    CANCELLED;      // Отменен

    fun getDisplayName(): String {
        return when (this) {
            PENDING -> "Ожидает обработки"
            CONFIRMED -> "Подтвержден"
            ASSIGNED -> "Назначен водитель"
            IN_TRANSIT -> "В пути"
            DELIVERED -> "Доставлен"
            CANCELLED -> "Отменен"
        }
    }

    fun getColor(): Long {
        return when (this) {
            PENDING -> 0xFFFFA726 // Orange
            CONFIRMED -> 0xFF42A5F5 // Blue
            ASSIGNED -> 0xFF26C6DA // Cyan
            IN_TRANSIT -> 0xFF9CCC65 // Light Green
            DELIVERED -> 0xFF66BB6A // Green
            CANCELLED -> 0xFFEF5350 // Red
        }
    }
}

/**
 * Модель водителя
 */
@Parcelize
data class Driver(
    val id: String,
    val name: String,
    val phone: String,
    val licenseNumber: String,
    val vehicleType: String,
    val vehicleNumber: String,
    val status: DriverStatus,
    val currentLocation: Location?
) : Parcelable

@Parcelize
enum class DriverStatus : Parcelable {
    AVAILABLE,
    ON_ROUTE,
    OFFLINE
}

/**
 * Модель местоположения
 */
@Parcelize
data class Location(
    val latitude: Double,
    val longitude: Double,
    val address: String
) : Parcelable

/**
 * Модель транспортного средства
 */
@Parcelize
data class Vehicle(
    val id: String,
    val number: String,
    val type: VehicleType,
    val brand: String,
    val model: String,
    val maxWeight: Double, // тонны
    val maxVolume: Double  // м³
) : Parcelable

@Parcelize
enum class VehicleType : Parcelable {
    TRUCK,
    VAN,
    CARGO_VAN,
    TRAILER;

    fun getDisplayName(): String {
        return when (this) {
            TRUCK -> "Грузовик"
            VAN -> "Фургон"
            CARGO_VAN -> "Грузовой фургон"
            TRAILER -> "Фура"
        }
    }
}

/**
 * Модель клиента
 */
@Parcelize
data class Client(
    val id: String,
    val name: String,
    val phone: String,
    val email: String,
    val company: String?,
    val address: String,
    val registrationDate: Date
) : Parcelable

/**
 * Модель отслеживания груза
 */
@Parcelize
data class TrackingInfo(
    val orderId: String,
    val currentLocation: Location,
    val lastUpdate: Date,
    val estimatedArrival: Date?,
    val statusHistory: List<StatusUpdate>
) : Parcelable

@Parcelize
data class StatusUpdate(
    val status: OrderStatus,
    val timestamp: Date,
    val location: Location?,
    val comment: String?
) : Parcelable
