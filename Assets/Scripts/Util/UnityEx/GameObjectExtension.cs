using System.Collections;
using UnityEngine;
using System.Collections.Generic;


namespace BeatsFever.UnityEx
{
    class CoroutineBehaviour : MonoBehaviour
    {
        public void Run(IEnumerator enumerator)
        {
            StartCoroutine(RunInternal(enumerator));
        }

        private IEnumerator RunInternal(IEnumerator enumerator)
        {
            yield return StartCoroutine(enumerator);
            Destroy(this);
        }
    }

    public static class GameObjectExtension
    {
        public static List<GameObject> GetAllChildren(this GameObject gameObject)
        {
            var children = new List<GameObject>();
            foreach (Transform transform in gameObject.transform)
            {
                children.Add(transform.gameObject);
            }

            return children;
        }


        public static void RemoveAllChildren(this GameObject gameObject)
        {
            gameObject.GetAllChildren().ForEach(obj => Object.Destroy(obj));
        }


        public static void RemoveChildrenWithComponent<T>(this GameObject gameObject, bool includeInactive = false) where T : Component
        {
			if(null == gameObject)
			{
				return;
			}

            var items = gameObject.GetComponentsInChildren<T>(includeInactive);
            foreach (var item in items)
            {
                Object.Destroy(item.gameObject);
            }
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static void SetLayerRecursively(this GameObject obj, int layer, int oldLayer)
        {
            if (obj.layer == oldLayer)
            {
                obj.layer = layer;
            }
            
            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer, oldLayer);
            }
        }

        public static void StartCoroutineExt(this GameObject obj, IEnumerator coroutine)
        {
            var behaviour = obj.AddComponent<CoroutineBehaviour>();
            behaviour.Run(coroutine);
        }


        public static T EnsureComponent<T>(this GameObject obj) where T : Component
        {
            var comp = obj.GetComponent<T>();
            if (comp == null)
            {
                comp = obj.AddComponent<T>();
            }

            return comp;
        }


        public static GameObject EnsureChildObject(this GameObject obj, string childName)
        {
            GameObject childObj = null;
            var childTrans = obj.transform.FindChild(childName);
            if (childTrans == null)
            {
                childObj = new GameObject(childName);
                childObj.transform.parent = obj.transform;
                childObj.transform.ResetLocalTRS();
            }
            else
            {
                childObj = childTrans.gameObject;
            }

            return childObj;
        }
    }

    public static class RectExtension
    {

        public static bool ContainsIncludeEdge(this Rect rect, Vector2 p)
        {
            return rect.xMin <= p.x && rect.xMax >= p.x && rect.yMin <= p.y && rect.yMax >= p.y;
        }

        public static bool ContainsExcludeEdge(this Rect rect, Vector2 p)
        {
            return rect.xMin < p.x && rect.xMax > p.x && rect.yMin < p.y && rect.yMax > p.y;
        }
    }
}
