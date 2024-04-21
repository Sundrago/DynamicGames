// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class Pagination : MonoBehaviour
    {
        public GameObject PaginationDotPrefab;
        public Color ActiveColor;
        public Color InactiveColor;
        protected List<Image> _images;

        public virtual void InitializePagination(int numberOfPages)
        {
            _images = new List<Image>();
            for (var i = 0; i < numberOfPages; i++)
            {
                var dotPrefab = Instantiate(PaginationDotPrefab);
                dotPrefab.transform.SetParent(transform);
                dotPrefab.name = "PaginationDot" + i;
                _images.Add(dotPrefab.GetComponent<Image>());
            }

            foreach (var image in _images)
            {
                image.color = InactiveColor;
                image.rectTransform.localScale = Vector3.one;
                image.rectTransform.localPosition = Vector3.zero;
                image.SetNativeSize();
            }
        }

        public virtual void SetCurrentPage(int numberOfPages, int currentPage)
        {
            for (var i = 0; i < numberOfPages; i++)
                if (i == currentPage)
                    _images[i].color = ActiveColor;
                else
                    _images[i].color = InactiveColor;
        }
    }
}