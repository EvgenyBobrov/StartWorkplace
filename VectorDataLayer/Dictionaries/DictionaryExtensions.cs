using System;

namespace VectorDataLayer
{
	public static class DictionaryExtensions
	{
		public static string GetWinchPositionName(this EnumWinchEmploeePosition position)
		{
			switch (position)
			{
			case EnumWinchEmploeePosition.WinchMaster:
				return "Оператор";

			case EnumWinchEmploeePosition.WinchAssistant:
				return "Стажер";

			case EnumWinchEmploeePosition.WinchSecondAssistant:
				return "Второй стажер";

			default:
				return "Должность не определена";
			}
		}

		public static string GetStartWorkerPositionName (this EnumStarterPosition position)
		{
			switch (position)
			{
			case EnumStarterPosition.TandemMaster:
				return "Тандеммастер";

			case EnumStarterPosition.Instructor:
				return "Инструктор";

			case EnumStarterPosition.Issuer:
				return "Выпускающий";

			case EnumStarterPosition.Assistant:
				return "Стажер";

			default:
				return "Должность не определена";
			}
		}

		public static EnumStarterPosition GetStarterPositionByName (string name)
		{
			switch (name)
			{
			case "Тандеммастер":
				return EnumStarterPosition.TandemMaster;

			case "Инструктор":
				return EnumStarterPosition.Instructor;

			case "Выпускающий":
				return EnumStarterPosition.Issuer;

			case "Стажер":
				return EnumStarterPosition.Assistant;

			default:
				throw new Exception(string.Format("Неизвестный тип работника старта {0}", name) );
			}
		}
	}
}

