using UnityEngine;

public class Interactable : MonoBehaviour
{

   protected MeshRenderer _meshRenderer;
   protected Material _defaultMaterial;
   [SerializeField]private Material highlightMaterial;
   private void Start()
   {
      UpdateMeshAndMaterial(GetComponentInChildren<MeshRenderer>());
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.B))
      {
         HighlightActive(true);
      }
   }

   protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
   {
      _meshRenderer = newMesh;
      _defaultMaterial = newMesh.sharedMaterial;
   }
   
   public virtual void Interaction()
   {
      print("Interaction With " + gameObject.name);
   }

   public void HighlightActive(bool active)
   {
      if (active)
         _meshRenderer.material = highlightMaterial;
      else
         _meshRenderer.material = _defaultMaterial;
   }
   
   protected virtual void OnTriggerEnter(Collider other)
   {
      PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

      if (playerInteraction == null) return;
      HighlightActive(true);
      playerInteraction.GetInteractables().Add(this);
      playerInteraction.UpdateClosestInteractable();
   }
   protected virtual void OnTriggerExit(Collider other)
   {
      PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

      if (playerInteraction == null) return;
      HighlightActive(false);
      playerInteraction.GetInteractables().Remove(this);
      playerInteraction.UpdateClosestInteractable();
   }
}
