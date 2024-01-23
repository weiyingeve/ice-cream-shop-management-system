using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//==========================================================
// Student Number : S10257093
// Student Name : Isabelle Tan
// Partner Name : Charlotte Lee
//==========================================================
namespace ice_cream_shop_management_system
{
    class PointCard
    {
        public int Points { get; set; }
        public int PunchCard { get; set; }
        public string Tier { get; set; }
        public PointCard() { }
        public PointCard(int points, int punchCard)
        {
            Points = points;
            PunchCard = punchCard;
        }
        public void AddPoints(int points)
        {
            Points += points;
        }
        public void RedeemPoints(int points)
        {
            Points -= points;
        }
        public void Punch()
        {
            if (PunchCard < 11)
            {
                PunchCard++;
            }
            else
            {
                PunchCard = 10;
            }
        }
        public override string ToString()
        {
            return "Points: " + Points + "\tPunch Card: " + PunchCard +
                "\tTier: " + Tier;
        }
    }
}
