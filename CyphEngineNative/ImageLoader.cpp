#include "ImageLoader.h"

#define STB_IMAGE_IMPLEMENTATION
#define STBI_WINDOWS_UTF8
#include <stb_image.h>

uint8_t* LoadImageNative(const char* filename, int& width, int& height, OriginalChannels& originalChannels, DesiredChannels desiredChannels)
{
    stbi_set_flip_vertically_on_load(true);
    return stbi_load(filename, &width, &height, reinterpret_cast<int*>(&originalChannels), static_cast<int>(desiredChannels));
}

void FreeImageNative(uint8_t* image)
{
    stbi_image_free(image);
}
