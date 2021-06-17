﻿using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PostManager : SingletonMono<PostManager>
{
    public class PostInfo
    {
        public ObscuredString Indate;
        public ObscuredInt itemCount;
        public ObscuredInt itemType;
        public ObscuredString titleText;
        public ObscuredString contentText;
    }

    private List<PostInfo> postList = new List<PostInfo>();
    public List<PostInfo> PostList => postList;

    public ReactiveCommand WhenPostRefreshed = new ReactiveCommand();


    //SDK 문서

    //https://developer.thebackend.io/unity3d/guide/social/GetPostListV2/
    //https://developer.thebackend.io/unity3d/guide/social/postv2/ReceiveAdminPostItemV2/

    public void RefreshPost(bool retry = false)
    {

        // example
        Backend.Social.Post.GetPostListV2((bro) =>
        {
            if (bro.IsSuccess())
            {
                postList.Clear();

                //우편 받았을때 데이터 파싱
                //
                var rows = bro.GetReturnValuetoJSON();

                if (rows.ContainsKey("fromAdmin"))
                {
                    var fromadmin = rows["fromAdmin"];

                    for (int i = 0; i < fromadmin.Count; i++)
                    {
                        var postInfo = fromadmin[i];

                        PostInfo post = new PostInfo();
                        post.Indate = postInfo["inDate"][DatabaseManager.format_string].ToString();
                        post.itemCount = int.Parse(postInfo["itemCount"][DatabaseManager.format_Number].ToString());
                        post.itemType = int.Parse(postInfo["item"][DatabaseManager.format_dic]["ItemType"][DatabaseManager.format_string].ToString());
                        post.titleText = postInfo["title"][DatabaseManager.format_string].ToString();
                        post.contentText = postInfo["content"][DatabaseManager.format_string].ToString();

                        postList.Add(post);
                    }
                }

                WhenPostRefreshed.Execute();
            }
            else
            {
                if (retry)
                {
                    PopupManager.Instance.ShowYesNoPopup("우편 갱신 실패", "재시도 합까요?", () =>
                     {
                         RefreshPost(retry);
                     }, null);
                }
            }
        });
    }

    public void ReceivePost(PostInfo post)
    {

        Backend.Social.Post.ReceiveAdminPostItemV2(post.Indate, (bro) =>
        {
            // 이후 처리
            if (bro.IsSuccess())
            {
                SoundManager.Instance.PlaySound("GoldUse");
                DatabaseManager.GetPostItem((Item_Type)((int)post.itemType), post.itemCount);
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "우편을 수령했습니다.", null);
                RefreshPost(true);
            }
            else
            {
                PopupManager.Instance.ShowYesNoPopup("수령 실패", "우편을 받지 못했습니다.\n다시 시도할까요?", () =>
                {
                    ReceivePost(post);
                }, null);
            }
        });

    }
}
