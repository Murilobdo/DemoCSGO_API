using DemoInfo;

namespace DemoCSGO.Shared.Interfaces
{
    public interface ICoreBase
    {
         string GetNameWeapon(EquipmentElement equipment);
         void WriteJsonFile(string nameFile, string json);
    }
}