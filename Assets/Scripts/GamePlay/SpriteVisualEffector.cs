using UnityEngine;

namespace AgeOfHeroes
{
    public class SpriteVisualEffector
    {
        public void CreatePlayerSelectionEffect(SpriteRenderer _spriteRenderer, PlayerColor playerColor)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            _spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetColor("_InnerOutlineColor", GlobalVariables.playerColors[playerColor]);
            _spriteRenderer.SetPropertyBlock(mpb);
        }

        public void RemovePlayerSelectionEffect(SpriteRenderer _spriteRenderer)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            _spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetColor("_InnerOutlineColor", Color.black);
            _spriteRenderer.SetPropertyBlock(mpb);
        }

        public void CreatePlayerMovementPointsEffect(SpriteRenderer _spriteRenderer)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            _spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_HsvSaturation", 0.2f);
            _spriteRenderer.SetPropertyBlock(mpb);
        }

        public void RemovePlayerMovementPointsEffect(SpriteRenderer _spriteRenderer)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            _spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_HsvSaturation", 1f);
            _spriteRenderer.SetPropertyBlock(mpb);
        }
    }
}