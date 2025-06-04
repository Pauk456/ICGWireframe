using System.Collections.Generic;

namespace ICGFrame.Domain.DTO;

public class Line3D
{
    public List<Point3D> Points { get; set; } = new();
}