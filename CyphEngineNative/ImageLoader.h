#pragma once
#include <cstdint>

enum class DesiredChannels
{
    DEFAULT = 0,
    GRAY = 1,
    GRAY_ALPHA = 2,
    RGB = 3,
    RGB_ALPHA = 4
};

enum class OriginalChannels
{
    GRAY = 1,
    GRAY_ALPHA = 2,
    RGB = 3,
    RGB_ALPHA = 4
};

extern "C"
{
    __declspec(dllexport) uint8_t* LoadImageNative(const char* filename, int& width, int& height, OriginalChannels& originalChannels, DesiredChannels desiredChannels);
    __declspec(dllexport) void FreeImageNative(uint8_t* image);
}
