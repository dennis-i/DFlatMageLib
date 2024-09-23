using DFlatMage.Enums;
using DFlatMage.Interfaces;

namespace Testing;

public abstract class TestBase
{
    public const string ArtifactsPath = "../../../Artifacts";

    protected void ImageSaveBmp(IImage img, string fileName)
    {
        if (!Directory.Exists(ArtifactsPath))
            Directory.CreateDirectory(ArtifactsPath);


        string filePath = Path.Combine(ArtifactsPath, fileName);
        img.Save(filePath, ImageFormatType.Bitmap);
    }
}