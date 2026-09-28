using System;
using System.IO;
using System.Linq;
using System.Drawing;
using SmartBank.Infrastructure.Logging;

namespace SmartBank.Business
{

    public enum ImageValidationError
    {
        None,
        FileNotFound,
        InvalidExtension,
        FileTooLarge,
        DimensionsTooSmall,
        InvalidImage
    }
    public static class ImageValidator
    {
        private const int MaxProfilePhotoSizeInBytes = 1024 * 1024 * 5; // 5MB
        private const int MinProfilePhotoWidth = 200;
        private const int MinProfilePhotoHeight = 200;
        private static readonly string[] AllowedProfilePhotoExtensions = { ".png", ".jpg", ".jpeg" };

        public static ImageValidationError Validate(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                return ImageValidationError.FileNotFound;
            }

            FileInfo fileInfo = new FileInfo(imagePath);

            if (!AllowedProfilePhotoExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
            {
                return ImageValidationError.InvalidExtension;
            }

            if (fileInfo.Length > MaxProfilePhotoSizeInBytes)
            {
                return ImageValidationError.FileTooLarge;
            }

            try
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    if (image.Height < MinProfilePhotoHeight || image.Width < MinProfilePhotoWidth)
                    {
                        return ImageValidationError.DimensionsTooSmall;
                    }
                }
            }
            catch (Exception ex)
            {
                EventViewerLogger.LogWarning(ex, "The image file could not be loaded during validation");

                return ImageValidationError.InvalidImage;
            }

            return ImageValidationError.None;
        }
    }
}
