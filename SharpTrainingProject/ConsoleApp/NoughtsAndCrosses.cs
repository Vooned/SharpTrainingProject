using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    public static class NoughtsAndCrosses
    {
        struct Field
        {
            public Field(int X, int Y)
            {
                PlayingField = new Coordinate[X, Y];
                CreateField();
            }

            Coordinate[,] PlayingField;

            /// <summary>
            /// Создаёт чистое поле
            /// </summary>
            private void CreateField()
            {
                for (int i = 0; i < PlayingField.GetLength(0); i++)
                    for (int j = 0; j < PlayingField.GetLength(1); j++)
                        PlayingField[i, j] = new Coordinate(false, false);
            }

            /// <summary>
            /// Отображает игровое поле на экране
            /// </summary>
            public void ShowField()
            {
                for (int i = 0; i < PlayingField.GetLength(0); i++)
                    for (int j = 0; j < PlayingField.GetLength(1); j++)
                    {
                        if (PlayingField[i, j].IsActive == true) Console.ForegroundColor = ConsoleColor.Red;
                        if (j != 2) Console.Write($"{PlayingField[i, j].CoordValue}");
                        else Console.WriteLine($"{PlayingField[i, j].CoordValue}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
            }

            /// <summary>
            /// Возвращает координаты активной клетки игрового поля
            /// </summary>
            /// <returns></returns>
            public (int, int) GetActiveCoordinate()
            {
                (int, int) Coord  = (0, 0);
                for (int i = 0; i < PlayingField.GetLength(0); i++)
                    for (int j = 0; j < PlayingField.GetLength(1); j++)
                        if (PlayingField[i, j].IsActive) Coord = (i, j);
                return Coord;
            }

            /// <summary>
            /// Устанавливает активную клетку
            /// </summary>
            /// <param name="X">Х-координата</param>
            /// <param name="Y">Y-координата</param>
            public void SetActiveCoordinate(int X, int Y)
            {
                (int, int) Coord = GetActiveCoordinate();
                PlayingField[Coord.Item1, Coord.Item2].IsActive = false;
                PlayingField[X, Y].IsActive = true;
            }

            /// <summary>
            /// Отмечает в клетке ход игрока
            /// </summary>
            /// <param name="Mark">Метка игрока</param>
            public bool MarkCoordinate(string Mark)
            {
                (int, int) Coord = GetActiveCoordinate();
                PlayingField[Coord.Item1, Coord.Item2].CoordValue = $"|_{Mark}_|";
                if (PlayingField[Coord.Item1, Coord.Item2].CoordValue.Contains(Mark))
                    return true;
                else return false;
            }

            /// <summary>
            /// Возвращает список координат всех клеток, где ходил заданный игрок
            /// </summary>
            /// <param name="Mark">Символ игрока</param>
            /// <returns></returns>
            public List<(int,int)> GetAllMarksCoord(string Mark)
            {
                List<(int, int)> Coord = new List<(int, int)>();
                for (int i = 0; i < PlayingField.GetLength(0); i++)
                    for (int j = 0; j < PlayingField.GetLength(1); j++)
                        if (PlayingField[i, j].CoordValue.Contains(Mark)) Coord.Add((i,j));
                return Coord;
            }

            /// <summary>
            /// Игровая клетка
            /// </summary>
            struct Coordinate
            {
                public Coordinate(bool IsActive, bool HasValue, string Value = "|___|")
                {
                    this.IsActive = IsActive;
                    this.HasValue = HasValue;
                    coordvalue = Value;
                }

                /// <summary>
                /// Выбрана ли клетка
                /// </summary>
                public bool IsActive { get; set; }

                /// <summary>
                /// Имеет ли клетка значение Х или О
                /// </summary>
                public bool HasValue { get; set; }

                private string coordvalue;
                /// <summary>
                /// Строковое представление клетки
                /// </summary>
                public string CoordValue
                {
                    get { return coordvalue; }
                    set
                    {
                        if (HasValue != true)
                        {
                            coordvalue = value;
                            HasValue = true;
                        }
                    }
                }
            }
        }

        static Field PlayingField = new Field(3,3);

        /// <summary>
        /// Игрок
        /// </summary>
        struct Player
        {
            public Player(string PlayerValue)
            {
                activeplayer = false;
                this.PlayerValue = PlayerValue;
            }

            private bool activeplayer;
            /// <summary>
            /// Текущий игрок (True - ходит Х, False - ходит О)
            /// </summary>
            public bool ActivePlayer 
            { get { return activeplayer; }
                set 
                {
                    if (value == true) PlayerValue = "X";
                    else PlayerValue = "O";
                    activeplayer = value;
                }
            }

            /// <summary>
            /// Строковое представление игрока
            /// </summary>
            public string PlayerValue { get; set; }

            /// <summary>
            /// Изменение активного игрока
            /// </summary>
            public void ChangePlayer()
            {
                if (ActivePlayer) ActivePlayer = false;
                else ActivePlayer = true;
            }
        }

        static Player CurrentPlayer = new Player("None");

        static string Winner = "";

        static ConsoleKeyInfo PressedKey;

        static List<(int, int)> WinningCoordinate;

        /// <summary>
        /// Запуск новой игры
        /// </summary>
        public static void StartGame()
        {
            WinningCoordinate = new List<(int, int)>();
            PlayingField.SetActiveCoordinate(0, 0);
            (int, int) ActiveCoordinate = (0, 0);
            //Определяем первого игрока
            {
                Random Rnd = new Random();
                if (Rnd.Next(0, 2) == 0)
                    CurrentPlayer.ActivePlayer = true;
                else CurrentPlayer.ActivePlayer = false;
            }
            //Пока не определён победитель, продолжать игру
            while (Winner == "")
            {
                WinningCoordinate.Clear();
                Console.Clear();
                Console.WriteLine($"Сейчас ходит: {CurrentPlayer.PlayerValue}");
                PlayingField.ShowField();
                PressedKey = Console.ReadKey();
                switch (PressedKey.Key)
                {
                    case ConsoleKey.LeftArrow: 
                        if (PlayingField.GetActiveCoordinate().Item2 != 0) 
                            PlayingField.SetActiveCoordinate(ActiveCoordinate.Item1, ActiveCoordinate.Item2 - 1); break;
                    case ConsoleKey.UpArrow:
                        if (PlayingField.GetActiveCoordinate().Item1 != 0)
                            PlayingField.SetActiveCoordinate(ActiveCoordinate.Item1 - 1, ActiveCoordinate.Item2); break;
                    case ConsoleKey.RightArrow:
                        if (PlayingField.GetActiveCoordinate().Item2 != 2)
                            PlayingField.SetActiveCoordinate(ActiveCoordinate.Item1, ActiveCoordinate.Item2 + 1); break;
                    case ConsoleKey.DownArrow:
                        if (PlayingField.GetActiveCoordinate().Item1 != 2)
                            PlayingField.SetActiveCoordinate(ActiveCoordinate.Item1 + 1, ActiveCoordinate.Item2); break;
                    case ConsoleKey.Enter:
                        if (PlayingField.MarkCoordinate(CurrentPlayer.PlayerValue))
                        {
                            //Определение победителя
                            WinningCoordinate.AddRange(PlayingField.GetAllMarksCoord(CurrentPlayer.PlayerValue));
                            for (int i = 0; i < 3; i++)
                            {
                                if (WinningCoordinate.FindAll(x => x.Item1 == i).Count == 3 ||
                                    WinningCoordinate.FindAll(x => x.Item2 == i).Count == 3 ||
                                    WinningCoordinate.FindAll(x => x.Item1 == x.Item2).Count == 3 ||
                                        (WinningCoordinate.FindAll(x => x.Item1 == 2 && x.Item2 == 0).Count +
                                        WinningCoordinate.FindAll(x => x.Item1 == 1 && x.Item2 == 1).Count +
                                        WinningCoordinate.FindAll(x => x.Item1 == 0 && x.Item2 == 2).Count) == 3)
                                { 
                                    Winner = CurrentPlayer.PlayerValue;
                                    break;
                                }
                            }
                            //Смена игрока
                            if (Winner == "") CurrentPlayer.ChangePlayer(); 
                        } break;
                }
                ActiveCoordinate = PlayingField.GetActiveCoordinate();
                //Если закончились свободные клетки, а победителя нет - конец игры
                if (WinningCoordinate.Count == 5 && Winner == "") break;
            }
            Console.Clear();
            PlayingField.ShowField();
            if (Winner != "")
                Console.WriteLine($"Победил игрок {CurrentPlayer.PlayerValue}!");
            else Console.WriteLine($"Ничья.");
        }

    }
}
