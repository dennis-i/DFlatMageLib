using DFlatMage.Interfaces;

namespace DFlatMage.Impl;


public class OutOfImageRangeException(string text) : Exception(text);
public class NotSupportedImageException(in IImage img) : Exception($"Image not supported,Planes:{img.NumPlanes},Bpp:{img.Bpp}");


