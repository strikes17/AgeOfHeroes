using System.Collections;

namespace AgeOfHeroes
{
    public interface IClickTarget
    {
        public IEnumerator OnClicked(Player player);
    }
}