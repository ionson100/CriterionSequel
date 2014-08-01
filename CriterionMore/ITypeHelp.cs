using Microsoft.Win32;

namespace CriterionMore
{
    /// <summary>
   /// ������ ��� �������� ������ ������� ������ ����������� ���� ���������
   /// </summary>
   public  interface ITypeHelp
    {
       /// <summary>
       ///��������� ������ ��� �������
       /// </summary>
       /// <param name="id"> ���������� ��������������</param>
       /// <returns>������ ��� ����������� �������</returns>
       string GetHelp(string id);
    }
}