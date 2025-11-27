// parent.cpp — ConsoleApplication2
#include <windows.h>
#include <string>
#include <iostream>
#include <sstream>
#include <iomanip>

static const SIZE_T BUFSZ = 4096;

// Генерация уникальных имён для объектов ядра (mapping, events)
std::string make_unique_name(const char* prefix) {
    std::ostringstream oss;
    DWORD t = GetTickCount();
    oss << prefix << "_" << t << "_" << GetCurrentProcessId();
    return oss.str();
}

int main() {
    std::string mappingName = make_unique_name("Local\\MySharedMap");
    std::string evParentName = make_unique_name("Local\\ParentReady");
    std::string evChildName = make_unique_name("Local\\ChildReady");

    // Создаём file mapping (разделяемую память)
    HANDLE hMap = CreateFileMappingA(
        INVALID_HANDLE_VALUE,
        nullptr,
        PAGE_READWRITE,
        0,
        (DWORD)BUFSZ,
        mappingName.c_str()
    );
    if (!hMap) {
        std::cerr << "CreateFileMapping failed: " << GetLastError() << "\n";
        return 1;
    }

    // Отображаем память
    LPVOID localView = MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, BUFSZ);
    if (!localView) {
        std::cerr << "MapViewOfFile (parent) failed: " << GetLastError() << "\n";
        CloseHandle(hMap);
        return 1;
    }

    std::uintptr_t preferredAddress = reinterpret_cast<std::uintptr_t>(localView);

    // Создаём события для синхронизации
    HANDLE hEventParent = CreateEventA(nullptr, FALSE, FALSE, evParentName.c_str());
    HANDLE hEventChild = CreateEventA(nullptr, FALSE, FALSE, evChildName.c_str());
    if (!hEventParent || !hEventChild) {
        std::cerr << "CreateEvent failed: " << GetLastError() << "\n";
        UnmapViewOfFile(localView);
        CloseHandle(hMap);
        return 1;
    }

    // === Определяем абсолютный путь к дочерней программе ===
    char parentPath[MAX_PATH];
    GetModuleFileNameA(NULL, parentPath, MAX_PATH);
    std::string dir(parentPath);
    dir = dir.substr(0, dir.find_last_of("\\/"));

    // Путь к дочернему exe (ConsoleApplication3)
    std::string childFullPath = dir + "\\ConsoleApplication3.exe";

    // Проверим, существует ли файл
    if (GetFileAttributesA(childFullPath.c_str()) == INVALID_FILE_ATTRIBUTES) {
        std::cerr << "Child executable not found: " << childFullPath << "\n";
        UnmapViewOfFile(localView);
        CloseHandle(hMap);
        return 1;
    }

    // Формируем командную строку для запуска дочернего процесса
    std::ostringstream cmd;
    cmd << "\"" << childFullPath << "\" "
        << "\"" << mappingName << "\" "
        << std::hex << "0x" << preferredAddress << " "
        << "\"" << evParentName << "\" "
        << "\"" << evChildName << "\"";

    std::string cmdstr = cmd.str();

    STARTUPINFOA si = { sizeof(si) };
    PROCESS_INFORMATION pi = { 0 };

    // Запускаем дочерний процесс
    BOOL ok = CreateProcessA(
        nullptr,
        &cmdstr[0],
        nullptr,
        nullptr,
        FALSE,
        CREATE_NEW_CONSOLE,
        nullptr,
        nullptr,
        &si,
        &pi
    );

    if (!ok) {
        std::cerr << "CreateProcess failed: " << GetLastError() << "\n";
        CloseHandle(hEventParent);
        CloseHandle(hEventChild);
        UnmapViewOfFile(localView);
        CloseHandle(hMap);
        return 1;
    }

    std::cout << "Parent: started child (PID=" << pi.dwProcessId << ")\n";
    std::cout << "Parent: mapping name = " << mappingName
        << ", preferred address = 0x" << std::hex << preferredAddress << std::dec << "\n";

    // Записываем сообщение
    const char* msg = "Hello from parent!";
    size_t msglen = strlen(msg) + 1;
    if (msglen > BUFSZ) msglen = BUFSZ;
    memcpy(localView, msg, msglen);

    // Сигнализируем дочернему процессу
    SetEvent(hEventParent);
    std::cout << "Parent: wrote message and signaled child.\n";

    // Ждём ответ
    DWORD wait = WaitForSingleObject(hEventChild, 10000);
    if (wait == WAIT_OBJECT_0) {
        char resp[BUFSZ];
        memcpy(resp, localView, BUFSZ);
        resp[BUFSZ - 1] = 0;
        std::cout << "Parent: received response from child: \"" << resp << "\"\n";
    }
    else if (wait == WAIT_TIMEOUT) {
        std::cerr << "Parent: timeout waiting for child.\n";
    }
    else {
        std::cerr << "Parent: WaitForSingleObject failed: " << GetLastError() << "\n";
    }

    // Очистка
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
    CloseHandle(hEventParent);
    CloseHandle(hEventChild);
    UnmapViewOfFile(localView);
    CloseHandle(hMap);

    return 0;
}
