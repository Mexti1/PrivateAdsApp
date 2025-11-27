package com.logistics.app.ui

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.navigation.NavDestination.Companion.hierarchy
import androidx.navigation.NavGraph.Companion.findStartDestination
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import com.logistics.app.data.model.Order
import com.logistics.app.ui.dashboard.DashboardScreen
import com.logistics.app.ui.orders.CreateOrderScreen
import com.logistics.app.ui.orders.OrderDetailScreen
import com.logistics.app.ui.orders.OrdersScreen
import com.logistics.app.ui.tracking.TrackingScreen

/**
 * Главный экран приложения с навигацией
 */
@Composable
fun MainScreen() {
    val navController = rememberNavController()
    var selectedOrder by remember { mutableStateOf<Order?>(null) }

    Scaffold(
        bottomBar = {
            BottomNavigationBar(navController = navController)
        }
    ) { paddingValues ->
        NavHost(
            navController = navController,
            startDestination = Screen.Dashboard.route,
            modifier = Modifier.padding(paddingValues)
        ) {
            composable(Screen.Dashboard.route) {
                DashboardScreen(
                    onViewOrdersClick = {
                        navController.navigate(Screen.Orders.route)
                    }
                )
            }

            composable(Screen.Orders.route) {
                OrdersScreen(
                    onOrderClick = { order ->
                        selectedOrder = order
                        navController.navigate(Screen.OrderDetail.route)
                    },
                    onCreateOrderClick = {
                        navController.navigate(Screen.CreateOrder.route)
                    }
                )
            }

            composable(Screen.OrderDetail.route) {
                selectedOrder?.let { order ->
                    OrderDetailScreen(
                        order = order,
                        onBackClick = { navController.navigateUp() },
                        onUpdateStatus = { newStatus ->
                            // Обновление статуса
                        }
                    )
                }
            }

            composable(Screen.CreateOrder.route) {
                CreateOrderScreen(
                    onBackClick = { navController.navigateUp() },
                    onOrderCreated = { order ->
                        navController.navigateUp()
                    }
                )
            }

            composable(Screen.Tracking.route) {
                TrackingScreen()
            }
        }
    }
}

/**
 * Нижняя панель навигации
 */
@Composable
fun BottomNavigationBar(navController: NavHostController) {
    val items = listOf(
        Screen.Dashboard,
        Screen.Orders,
        Screen.Tracking
    )

    NavigationBar(
        containerColor = MaterialTheme.colorScheme.surface,
        contentColor = MaterialTheme.colorScheme.primary
    ) {
        val navBackStackEntry by navController.currentBackStackEntryAsState()
        val currentDestination = navBackStackEntry?.destination

        items.forEach { screen ->
            NavigationBarItem(
                icon = {
                    Icon(
                        screen.icon,
                        contentDescription = screen.title
                    )
                },
                label = { Text(screen.title) },
                selected = currentDestination?.hierarchy?.any { 
                    it.route == screen.route 
                } == true,
                onClick = {
                    navController.navigate(screen.route) {
                        popUpTo(navController.graph.findStartDestination().id) {
                            saveState = true
                        }
                        launchSingleTop = true
                        restoreState = true
                    }
                },
                colors = NavigationBarItemDefaults.colors(
                    selectedIconColor = MaterialTheme.colorScheme.primary,
                    selectedTextColor = MaterialTheme.colorScheme.primary,
                    unselectedIconColor = Color.Gray,
                    unselectedTextColor = Color.Gray,
                    indicatorColor = MaterialTheme.colorScheme.primaryContainer
                )
            )
        }
    }
}

/**
 * Определение экранов приложения
 */
sealed class Screen(
    val route: String,
    val title: String,
    val icon: ImageVector
) {
    object Dashboard : Screen(
        route = "dashboard",
        title = "Главная",
        icon = Icons.Default.Home
    )

    object Orders : Screen(
        route = "orders",
        title = "Заказы",
        icon = Icons.Default.ShoppingCart
    )

    object OrderDetail : Screen(
        route = "order_detail",
        title = "Детали заказа",
        icon = Icons.Default.Info
    )

    object CreateOrder : Screen(
        route = "create_order",
        title = "Новый заказ",
        icon = Icons.Default.Add
    )

    object Tracking : Screen(
        route = "tracking",
        title = "Отслеживание",
        icon = Icons.Default.Place
    )
}
