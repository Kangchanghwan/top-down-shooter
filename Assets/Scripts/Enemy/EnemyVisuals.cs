using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyVisuals : MonoBehaviour
{
   [Header("Color")]
   [SerializeField] private Texture[] colorTextures;
   [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

   private void Start()
   {
      InvokeRepeating(nameof(SetupLook), 0, 1.5f);
   }

   public void SetupLook()
   {
      SetupRandomColor();
   }
   
   private void SetupRandomColor()
   {
      int randomIndex = Random.Range(0, colorTextures.Length);

      Material newMAt = new Material(skinnedMeshRenderer.material);

      newMAt.mainTexture = colorTextures[randomIndex];
      skinnedMeshRenderer.material = newMAt;
   }
}
