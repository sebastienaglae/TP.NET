#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define EXPORT __attribute__((visibility("default")))

// delegate* unmanaged<IntPtr, char*, char*, delegate* unmanaged<UnmanagedHttpRequest*, UnmanagedHttpResponse*>, void>
typedef int (*http_map_func)(void*, char*, char*, void*);

struct ByteArray {
    const char* data;
    int length;
};
struct UnmanagedHeader {
    struct ByteArray key;
    struct ByteArray value;
};
struct UnmanagedHttpResponse {
    int statusCode;
    struct ByteArray body;
    struct UnmanagedHeader* headers;
    int headerCount;
};
struct UnmanagedHttpRequest {
    struct ByteArray method;
    struct ByteArray path;
    struct ByteArray body;
};

struct ByteArray byte_array_from_cstring(const char* str) {
    struct ByteArray result;
    int length = strlen(str);
    result.data = (char*)malloc(length);
    memcpy(result.data, str, length);
    result.length = length;
    return result;
}

struct ByteArray byte_array_from_malloc(const char* data, int length) {
    struct ByteArray result;
    result.data = data;
    result.length = length;
    return result;
}

int hello_get(struct UnmanagedHttpRequest *request, struct UnmanagedHttpResponse* response) {
    response->statusCode = 200;
    response->body = byte_array_from_cstring("Hello from C!");

    struct UnmanagedHeader* headers = (struct UnmanagedHeader*)malloc(sizeof(struct UnmanagedHeader) * 2);
    headers[0].key = byte_array_from_cstring("Content-Type");
    headers[0].value = byte_array_from_cstring("text/plain");
    headers[1].key = byte_array_from_cstring("X-Test-Header");
    headers[1].value = byte_array_from_cstring("Hello from C!");
    response->headers = headers;
    response->headerCount = 2;

    return 0;
}

int hello_post(struct UnmanagedHttpRequest *request, struct UnmanagedHttpResponse* response) {
    // Hello from C!\n Your body was: {request->body.data}
    response->statusCode = 200;
    char buffer[1024];
    snprintf(buffer, 1024, "Hello from C!\n Your body was: %.*s", request->body.length, request->body.data);
    response->body = byte_array_from_cstring(buffer);

    struct UnmanagedHeader* headers = (struct UnmanagedHeader*)malloc(sizeof(struct UnmanagedHeader) * 2);
    headers[0].key = byte_array_from_cstring("Content-Type");
    headers[0].value = byte_array_from_cstring("text/plain");
    headers[1].key = byte_array_from_cstring("X-Test-Header");
    headers[1].value = byte_array_from_cstring("Hello from C! (POST)");
    response->headers = headers;
    response->headerCount = 2;

    return 0;
}

EXPORT int CreatePlugin(void *handle, http_map_func httpmap) {
    httpmap(handle, "GET", "/native/hello", hello_get);
    httpmap(handle, "POST", "/native/hello", hello_post);

    return 0;
}