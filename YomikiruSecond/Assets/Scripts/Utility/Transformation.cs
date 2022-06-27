using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Yomikiru.Utility
{
    [Serializable]
    public class TransformState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformState() : this(null) {}

        public TransformState(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        public TransformState(Transform transform)
        {
            if (transform is null)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                scale = Vector3.zero;
            }
            else
            {
                position = transform.position;
                rotation = transform.rotation;
                scale = transform.localScale;
            }
        }

        public static TransformState operator +(TransformState state1, TransformState state2)
        {
            return new TransformState(
                state1.position + state2.position,
                state1.rotation * state2.rotation,
                new Vector3(
                    state1.scale.x * state2.scale.x,
                    state1.scale.y * state2.scale.y,
                    state1.scale.z * state2.scale.z ));
        }

        public static void Copy(Transform dst, TransformState src)
        {
            dst.position = src.position;
            dst.rotation = src.rotation;
            dst.localScale = src.scale;
        }
        public static void Copy(TransformState dst, Transform src)
        {
            dst.position = src.position;
            dst.rotation = src.rotation;
            dst.scale = src.localScale;
        }
    };

    [Serializable]
    public struct TransformationData
    {
        public TransformState start;
        public TransformState end;
        public float time;
        public float delay;
    };

    public class Transformation : MonoBehaviour
    {
        public TransformationData data { get; set; }

        public async UniTask DoTransform(Ease ease = Ease.Linear, bool isReset = false, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            bool canceled = false;
            if (data.delay > 0.0f)
            {
                canceled = await UniTask.Delay((int)(data.delay * 1000.0f), cancellationToken: ct).SuppressCancellationThrow();
            }
            if (canceled is true) return;

            var before = new TransformState(transform);
            var start = before + data.start;
            var end = before + data.end;

            TransformState.Copy(transform, start);

            List<Tween> list = new List<Tween>();

            if (start.position != end.position)
            {
                list.Add(transform.DOMove(end.position, data.time).SetEase(ease).SetLink(this.gameObject));
            }
            if (start.rotation != end.rotation)
            {
                list.Add(transform.DORotateQuaternion(end.rotation, data.time).SetEase(ease).SetLink(this.gameObject));
            }
            if (start.scale != end.scale)
            {
                list.Add(transform.DOScale(end.scale, data.time).SetEase(ease).SetLink(this.gameObject));
            }

            if (list.Count <= 0) return;

            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    _ = sequence.Append(list[i]);
                }
                else
                {
                    _ = sequence.Join(list[i]);
                }
            }
            canceled = await sequence.WithCancellation(ct).SuppressCancellationThrow();
            if (canceled is true) return;

            if (isReset is false) return;
            TransformState.Copy(transform, start);
        }
    }
}

