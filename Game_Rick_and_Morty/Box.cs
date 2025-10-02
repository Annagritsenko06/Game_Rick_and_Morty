using System;

namespace Game_Rick_and_Morty
{
    internal class Box
    {
        public int Id { get; }
        public bool HasPortalgun { get; set; }
        public bool IsOpen { get; set; }

        public Box(int id)
        {
            Id = id;
            HasPortalgun = false;
            IsOpen = false;
        }

        public void Open()
        {
            IsOpen = true;
        }
    }
}
