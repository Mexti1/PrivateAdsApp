// child.cpp — ConsoleApplication3
#include <windows.h>
#include <string>
#include <iostream>
#include <sstream>
#include <iomanip>

static const SIZE_T BUFSZ = 4096;

int main(int argc, char** argv) {
    if (argc < 5) {
        std::cerr << "Usage: ConsoleApplication3.exe \"mappingName\" 0xPreferredAddress \"evParentName\" \"evChildName\"\n";
        return 1;
    }

    std::string mappingName = argv[1];
    std::string addrStr = argv[2];
    std::string evParentName = argv[3];
    std::string evChildName = argv[4];

    // Парсим адрес
    std::uintptr_t preferredAddr = 0;
    {
        std::istringstream iss(addrStr);
        if (addrStr.rfind("0x", 0) == 0 || addrStr.rfind("0X", 0) == 0)
            iss >> std::hex >> preferredAddr;
        else
            iss >> preferredAddr;
    }

    std::cout << "Child: mappingName=" << mappingName
        << ", preferredAddr=0x" << std::hex << preferredAddr << std::dec << "\n";

    // Открываем mapping
    HANDLE hMap = OpenFileMappingA(FILE_MAP_ALL_ACCESS, FALSE, mappingName.c_str());
    if (!hMap) {
        std::cerr << "Child: OpenFileMapping failed: " << GetLastError() << "\n";
        return 1;
    }

    // Отображаем по переданному адресу (если возможно)
    LPVOID view = nullptr;
    if (preferredAddr != 0) {
        view = MapViewOfFileEx(hMap, FILE_MAP_ALL_ACCESS, 0, 0, BUFSZ, reinterpret_cast<LPVOID>(preferredAddr));
        if (!view) {
            view = MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, BUFSZ);
            if (!view) {
                std::cerr << "Child: MapViewOfFile failed: " << GetLastError() << "\n";
                CloseHandle(hMap);
                return 1;
            }
            std::cout << "Child: MapViewOfFileEx failed, fallback to " << view << "\n";
        }
        else {
            std::cout << "Child: mapped at requested address: " << view << "\n";
        }
    }
    else {
        view = MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, BUFSZ);
        if (!view) {
            std::cerr << "Child: MapViewOfFile failed: " << GetLastError() << "\n";
            CloseHandle(hMap);
            return 1;
        }
    }

    // Открываем события
    HANDLE hEventParent = OpenEventA(SYNCHRONIZE, FALSE, evParentName.c_str());
    HANDLE hEventChild = OpenEventA(EVENT_MODIFY_STATE, FALSE, evChildName.c_str());
    if (!hEventParent || !hEventChild) {
        std::cerr << "Child: OpenEvent failed: " << GetLastError() << "\n";
        if (hEventParent) CloseHandle(hEventParent);
        if (hEventChild) CloseHandle(hEventChild);
        UnmapViewOfFile(view);
        CloseHandle(hMap);
        return 1;
    }

    std::cout << "Child: waiting for parent's message...\n";
    DWORD w = WaitForSingleObject(hEventParent, 10000);
    if (w != WAIT_OBJECT_0) {
        std::cerr << "Child: wait failed or timeout: " << GetLastError() << "\n";
        CloseHandle(hEventParent);
        CloseHandle(hEventChild);
        UnmapViewOfFile(view);
        CloseHandle(hMap);
        return 1;
    }

    // Читаем сообщение
    char buf[BUFSZ];
    memcpy(buf, view, BUFSZ);
    buf[BUFSZ - 1] = 0;
    std::cout << "Child: got message: \"" << buf << "\"\n";

    // Формируем ответ
    std::string reply = std::string("Hello parent, got your message: \"") + buf + "\"";
    size_t tocopy = reply.size() + 1;
    if (tocopy > BUFSZ) tocopy = BUFSZ;
    memcpy(view, reply.c_str(), tocopy);

    // Сигнализируем родителю
    SetEvent(hEventChild);
    std::cout << "Child: sent reply to parent.\n";

    // Очистка
    CloseHandle(hEventParent);
    CloseHandle(hEventChild);
    UnmapViewOfFile(view);
    CloseHandle(hMap);

    return 0;
}
