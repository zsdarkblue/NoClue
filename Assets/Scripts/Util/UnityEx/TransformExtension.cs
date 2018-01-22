using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BeatsFever.UnityEx
{
    public static class TransformExtension
    {
        public static void ResetLocalTRS(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void ResetTRS(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Get the first ancestor of a particular type
        /// </summary>
        /// <returns>
        /// The instance of the type or null
        /// </returns>
        /// <param name='gameObject'>
        /// The game object
        /// </param>
        /// <typeparam name='T'>
        /// The type to get 
        /// </typeparam>
        public static T FirstAncestorOfType<T>(this GameObject gameObject) where T : Component
        {
            var t = gameObject.transform.parent;
            T component = null;
            while (t != null && (component = t.GetComponent<T>()) == null)
            {
                t = t.parent;
            }
            return component;
        }
        
        /// <summary>
        /// Get the last ancestor of a particular type
        /// </summary>
        /// <returns>
        /// The instance of the type or null
        /// </returns>
        /// <param name='gameObject'>
        /// The game object
        /// </param>
        /// <typeparam name='T'>
        /// The type to get 
        /// </typeparam>
//        public static T LastAncestorOfType<T>(this GameObject gameObject) where T : class
//        {
//            var t = gameObject.transform.parent;
//            T component = null;
//            while (t != null)
//            {
//                var c = t.gameObject.FindImplementor<T>();
//                if (c != null)
//                {
//                    component = c;
//                }
//                t = t.parent;
//            }
//            return component;
//        }
//        
        
        public static T[] GetAllComponentsInChildren<T>(this GameObject go) where T : Component
        {
            return go.transform.GetComponentsInChildren<T>(true);
        }
    }
}
