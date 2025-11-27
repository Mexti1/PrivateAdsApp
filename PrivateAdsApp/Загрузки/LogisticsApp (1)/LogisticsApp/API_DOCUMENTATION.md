# API Documentation - Logistics App Backend

## Общая информация

Это документация описывает необходимые API endpoints для интеграции мобильного приложения с backend-сервером.

**Base URL:** `https://api.logistics-app.com/v1`

**Authentication:** Bearer Token (JWT)

## Endpoints

### Authentication

#### POST /auth/login
Авторизация пользователя

**Request:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "user_123",
    "name": "Иван Иванов",
    "role": "manager"
  }
}
```

#### POST /auth/register
Регистрация нового пользователя

**Request:**
```json
{
  "name": "Иван Иванов",
  "email": "user@example.com",
  "password": "password123",
  "phone": "+79991234567",
  "role": "manager"
}
```

---

### Orders

#### GET /orders
Получить список всех заказов

**Query Parameters:**
- `status` (optional) - фильтр по статусу (PENDING, CONFIRMED, ASSIGNED, IN_TRANSIT, DELIVERED, CANCELLED)
- `page` (optional) - номер страницы (default: 1)
- `limit` (optional) - количество на странице (default: 20)

**Response:**
```json
{
  "data": [
    {
      "id": "order_123",
      "orderNumber": "ORD-2025-001",
      "clientName": "ООО 'Компания'",
      "clientPhone": "+79991234567",
      "pickupAddress": "г. Москва, ул. Примерная, д. 1",
      "deliveryAddress": "г. СПб, пр. Невский, д. 100",
      "cargoDescription": "Электроника",
      "weight": 250.0,
      "volume": 2.5,
      "status": "IN_TRANSIT",
      "createdDate": "2025-01-15T10:30:00Z",
      "estimatedDeliveryDate": "2025-01-18T14:00:00Z",
      "actualDeliveryDate": null,
      "price": 25000.0,
      "driverId": "driver_456",
      "driverName": "Петров Петр",
      "vehicleNumber": "А123БВ777",
      "trackingNumber": "TRK2025001",
      "notes": "Хрупкий груз"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 150,
    "pages": 8
  }
}
```

#### GET /orders/{id}
Получить детальную информацию о заказе

**Response:**
```json
{
  "id": "order_123",
  "orderNumber": "ORD-2025-001",
  "clientName": "ООО 'Компания'",
  "clientPhone": "+79991234567",
  "pickupAddress": "г. Москва, ул. Примерная, д. 1",
  "deliveryAddress": "г. СПб, пр. Невский, д. 100",
  "cargoDescription": "Электроника",
  "weight": 250.0,
  "volume": 2.5,
  "status": "IN_TRANSIT",
  "createdDate": "2025-01-15T10:30:00Z",
  "estimatedDeliveryDate": "2025-01-18T14:00:00Z",
  "actualDeliveryDate": null,
  "price": 25000.0,
  "driverId": "driver_456",
  "driverName": "Петров Петр",
  "vehicleNumber": "А123БВ777",
  "trackingNumber": "TRK2025001",
  "notes": "Хрупкий груз"
}
```

#### POST /orders
Создать новый заказ

**Request:**
```json
{
  "clientName": "ООО 'Компания'",
  "clientPhone": "+79991234567",
  "pickupAddress": "г. Москва, ул. Примерная, д. 1",
  "deliveryAddress": "г. СПб, пр. Невский, д. 100",
  "cargoDescription": "Электроника",
  "weight": 250.0,
  "volume": 2.5,
  "price": 25000.0,
  "estimatedDays": 3,
  "notes": "Хрупкий груз"
}
```

**Response:**
```json
{
  "id": "order_123",
  "orderNumber": "ORD-2025-001",
  "trackingNumber": "TRK2025001",
  "status": "PENDING",
  "createdDate": "2025-01-15T10:30:00Z"
}
```

#### PATCH /orders/{id}/status
Обновить статус заказа

**Request:**
```json
{
  "status": "IN_TRANSIT",
  "comment": "Груз отправлен"
}
```

**Response:**
```json
{
  "id": "order_123",
  "status": "IN_TRANSIT",
  "updatedAt": "2025-01-15T12:00:00Z"
}
```

#### PATCH /orders/{id}/assign-driver
Назначить водителя на заказ

**Request:**
```json
{
  "driverId": "driver_456",
  "vehicleId": "vehicle_789"
}
```

**Response:**
```json
{
  "id": "order_123",
  "driverId": "driver_456",
  "driverName": "Петров Петр",
  "vehicleNumber": "А123БВ777",
  "status": "ASSIGNED"
}
```

---

### Tracking

#### GET /tracking/{trackingNumber}
Отследить груз по номеру

**Response:**
```json
{
  "orderId": "order_123",
  "trackingNumber": "TRK2025001",
  "currentLocation": {
    "latitude": 55.7558,
    "longitude": 37.6173,
    "address": "г. Москва, МКАД 50км"
  },
  "lastUpdate": "2025-01-16T08:30:00Z",
  "estimatedArrival": "2025-01-18T14:00:00Z",
  "statusHistory": [
    {
      "status": "PENDING",
      "timestamp": "2025-01-15T10:30:00Z",
      "location": {
        "latitude": 55.7558,
        "longitude": 37.6173,
        "address": "г. Москва, ул. Примерная, д. 1"
      },
      "comment": "Заказ создан"
    },
    {
      "status": "CONFIRMED",
      "timestamp": "2025-01-15T11:00:00Z",
      "location": {
        "latitude": 55.7558,
        "longitude": 37.6173,
        "address": "г. Москва, ул. Примерная, д. 1"
      },
      "comment": "Заказ подтвержден"
    },
    {
      "status": "IN_TRANSIT",
      "timestamp": "2025-01-15T14:00:00Z",
      "location": {
        "latitude": 55.7558,
        "longitude": 37.6173,
        "address": "г. Москва, МКАД"
      },
      "comment": "Груз в пути"
    }
  ]
}
```

---

### Statistics

#### GET /statistics
Получить общую статистику

**Response:**
```json
{
  "totalOrders": 150,
  "pendingOrders": 15,
  "confirmedOrders": 20,
  "assignedOrders": 10,
  "inTransitOrders": 25,
  "deliveredOrders": 75,
  "cancelledOrders": 5,
  "totalRevenue": 3750000.0,
  "monthlyRevenue": 450000.0
}
```

---

### Drivers

#### GET /drivers
Получить список водителей

**Response:**
```json
{
  "data": [
    {
      "id": "driver_456",
      "name": "Петров Петр Петрович",
      "phone": "+79991234567",
      "licenseNumber": "77 АВ 123456",
      "vehicleType": "TRUCK",
      "vehicleNumber": "А123БВ777",
      "status": "AVAILABLE",
      "currentLocation": {
        "latitude": 55.7558,
        "longitude": 37.6173,
        "address": "г. Москва"
      }
    }
  ]
}
```

#### GET /drivers/{id}
Получить информацию о водителе

#### POST /drivers
Добавить нового водителя

#### PATCH /drivers/{id}
Обновить информацию о водителе

---

### Vehicles

#### GET /vehicles
Получить список транспортных средств

**Response:**
```json
{
  "data": [
    {
      "id": "vehicle_789",
      "number": "А123БВ777",
      "type": "TRUCK",
      "brand": "КАМАЗ",
      "model": "65117",
      "maxWeight": 20.0,
      "maxVolume": 36.0,
      "status": "AVAILABLE"
    }
  ]
}
```

---

## Error Responses

Все ошибки возвращаются в следующем формате:

```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Описание ошибки",
    "details": {}
  }
}
```

### HTTP Status Codes:
- `200` - Успешный запрос
- `201` - Ресурс создан
- `400` - Неверный запрос
- `401` - Не авторизован
- `403` - Доступ запрещен
- `404` - Ресурс не найден
- `422` - Ошибка валидации
- `500` - Внутренняя ошибка сервера

### Error Codes:
- `INVALID_REQUEST` - Неверный формат запроса
- `UNAUTHORIZED` - Требуется авторизация
- `FORBIDDEN` - Недостаточно прав
- `NOT_FOUND` - Ресурс не найден
- `VALIDATION_ERROR` - Ошибка валидации данных
- `INTERNAL_ERROR` - Внутренняя ошибка

## Headers

Все запросы должны содержать следующие заголовки:

```
Authorization: Bearer {token}
Content-Type: application/json
Accept: application/json
```

## Rate Limiting

- 1000 запросов в час для авторизованных пользователей
- 100 запросов в час для неавторизованных запросов

## Pagination

Для списочных запросов используется пагинация:

**Query Parameters:**
- `page` - номер страницы (начиная с 1)
- `limit` - количество элементов на странице (max: 100)

**Response включает:**
```json
{
  "data": [...],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 150,
    "pages": 8
  }
}
```

## Webhooks

Для получения уведомлений о событиях можно настроить webhooks:

### События:
- `order.created` - создан новый заказ
- `order.status_changed` - изменен статус заказа
- `order.assigned` - назначен водитель
- `order.delivered` - заказ доставлен

### Формат webhook payload:
```json
{
  "event": "order.status_changed",
  "timestamp": "2025-01-15T10:30:00Z",
  "data": {
    "orderId": "order_123",
    "oldStatus": "CONFIRMED",
    "newStatus": "IN_TRANSIT"
  }
}
```

## Версионирование

API использует версионирование через URL: `/v1/`, `/v2/`, и т.д.

Текущая версия: `v1`

## Поддержка

Для технической поддержки по API:
- Email: api-support@logistics-app.com
- Документация: https://docs.logistics-app.com
- Статус API: https://status.logistics-app.com
