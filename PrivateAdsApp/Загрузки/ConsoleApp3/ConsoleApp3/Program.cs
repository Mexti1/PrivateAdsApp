#include <iostream>
#include <windows.h>

int main()
{
    // Размер блока виртуальной памяти
    const SIZE_T size = 1024; // 1 КБ

    // 1. Выделяем блок виртуальной памяти
    LPVOID lpBase = VirtualAlloc(
        NULL,           // Система выберет адрес
        size,           // Размер области
        MEM_COMMIT | MEM_RESERVE, // Выделить и зарезервировать
        PAGE_READWRITE  // Разрешаем чтение и запись
    );

    if (lpBase == NULL)
    {
        std::cerr << "Ошибка VirtualAlloc: " << GetLastError() << std::endl;
        return 1;
    }

    std::cout << "Блок памяти выделен по адресу: " << lpBase << std::endl;

    // 2. Заполняем блок символом 'A' (код 0x41)
    FillMemory(lpBase, size, 0x41);
    std::cout << "Блок памяти заполнен символом 'A'." << std::endl;

    // 3. Меняем атрибуты доступа (делаем только для чтения)
    DWORD oldProtect;
    if (VirtualProtect(lpBase, size, PAGE_READONLY, &oldProtect))
    {
        std::cout << "Атрибут доступа изменён на только чтение." << std::endl;
    }
    else
    {
        std::cerr << "Ошибка VirtualProtect: " << GetLastError() << std::endl;
    }

    // 4. Создаём второй блок и копируем туда данные
    LPVOID lpCopy = VirtualAlloc(NULL, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
    if (lpCopy == NULL)
    {
        std::cerr << "Ошибка VirtualAlloc для копии: " << GetLastError() << std::endl;
        VirtualFree(lpBase, 0, MEM_RELEASE);
        return 1;
    }

    CopyMemory(lpCopy, lpBase, size);
    std::cout << "Содержимое скопировано во второй блок." << std::endl;

    // 5. Очищаем оригинальный блок (обнуляем)
    // Нужно вернуть права на запись
    VirtualProtect(lpBase, size, PAGE_READWRITE, &oldProtect);
    ZeroMemory(lpBase, size);
    std::cout << "Оригинальный блок обнулён." << std::endl;

    // 6. Освобождаем оба блока памяти
    VirtualFree(lpBase, 0, MEM_RELEASE);
    VirtualFree(lpCopy, 0, MEM_RELEASE);

    std::cout << "Блоки памяти освобождены." << std::endl;

    return 0;
}
