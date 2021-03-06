﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinHoweGameTree
{
    public enum Around
    {
        我方,敌方
    }
    public class GameManager : UnityComponentSingleton<GameManager>
    {
        public GameObject piecePrefab;
        public Sprite Black, White,Simple;
        public Sprite AISprite, PlaySprite;
        private const float xoffset = 52.5f, yoffset = 39.5f;
        private piece[,] allPiece = new piece[15, 15];
        public int[,] lazis = new int[15, 15];

        private Around around = Around.我方;
        private Stack<Vector2Int> laziStack = new Stack<Vector2Int>();
        public Around Around
        {
            get
            {
                return around;
            }
        }
        private gobangAI ai = new gobangAI();
        private void Awake()
        {
            Init();
        }
        private void Init()
        {
            AISprite = White;
            PlaySprite = Black;
            laziStack = new Stack<Vector2Int>();
            around = Around.我方;
            allPiece[0, 0] = piecePrefab.GetComponent<piece>();
            Vector2 zero = piecePrefab.GetComponent<RectTransform>().anchoredPosition;
            for (int i = 0; i < 15; ++i)
            {
                for (int j = 0; j < 15; ++j)
                {
                    if (null == allPiece[i, j])
                    {
                        
                        if(null == allPiece[i, j])
                        {
                            GameObject newpiece = Instantiate(piecePrefab, transform);
                            allPiece[i, j] = newpiece.GetComponent<piece>();
                            newpiece.GetComponent<RectTransform>().anchoredPosition =
                            new Vector2(zero.x + xoffset * i, zero.y - yoffset * j);
                            allPiece[i, j].Record(i, j);
                            
                        }
                        

                    }
                    allPiece[i, j].UnShow();
                    lazis[i, j] = 0;
                }
            }
        }
        public void TurnAround(Vector2Int lazi)
        {
            laziStack.Push(lazi);
            var e = new Evaluation();
            e.Evaluate(lazis);
            if (e.lose)
            {
                if (Black == AISprite)
                    Debug.Log("游戏结束,你输了");
                else
                    Debug.Log("游戏结束,你赢了");
                around = Around.敌方;
                return;
            }
            if(e.win)
            {
                if(White == AISprite)
                    Debug.Log("游戏结束,你输了");
                else
                    Debug.Log("游戏结束,你赢了");
                around = Around.敌方;
                return;
            }
            
            if (Around.我方 == around)
            {
                around = Around.敌方;
                AI();
            }
                
            if (Around.敌方 == around)
                around = Around.我方;
        }
        private void AI()
        {
            if (Around.我方 == around) return;


            Vector2Int res = ai.AILazi(lazis);
            allPiece[res.x, res.y].Chess();
            
        }

        /// <summary>
        /// 检查是否结束游戏
        /// </summary>
        /// <param name="board">棋盘数据</param>
        /// <param name="lazi">落子信息</param>
        /// <param name="isPlay">是否是玩家</param>
        /// <returns></returns>
        public void CheckGameOver(Vector2Int lazi, int isPlay)
        {
            //四个方向计数 横 竖 左斜 右斜
            var direction = Evaluation.direction;
            //Lazi[][] direction = new[]
            //{
            //    new[]{ new Lazi(-1, 0), new Lazi(1, 0) },
            //    new[]{ new Lazi(0 ,-1), new Lazi(0, 1) },
            //    new[]{ new Lazi(-1, 1), new Lazi(1,-1) },
            //    new[]{ new Lazi(-1,-1), new Lazi(1, 1) }
            //};
            var winer = new[] {"玩家","None","AI" };
            foreach (var axis in direction)
            {
                int axis_count = 1;

                axis_count += CountOnDirection(lazi, isPlay, axis[0].x, axis[0].y);
                axis_count += CountOnDirection(lazi, isPlay, axis[1].x, axis[1].y);
                if (axis_count >= 5)
                {
                    Debug.Log("游戏结束,胜利者是"+ winer[isPlay+1]);
                }
            }
            
        }

        /// <summary>
        /// 计算一个方向上的连续棋子棋子数
        /// </summary>
        /// <param name="lazi"></param>
        /// <param name="isplay"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int CountOnDirection(Vector2 lazi, int isplay, int x, int y)
        {
            int i = (int)lazi.x;
            int j = (int)lazi.y;
            int count = 1;
            for (int k = 0; k < 5; ++k)
            {
                if (x != 0 && (j + x * k < 0 || j + x * k >= 5))
                    break;
                if (y != 0 && (i + y * k < 0 || i + y * k >= 5))
                    break;
                if (isplay == lazis[i + y * k, j + x * k])
                    count += 1;
                else
                    break;
            }

            return count;

        }

        [ContextMenu("悔棋")]
        public void Cheki()
        {
            for(int i =0;i<2;++i)
            {
                var e = laziStack.Pop();
                allPiece[e.x, e.y].UnShow();
            }
            

        }
        [ContextMenu("重新开始(玩家先手)")]
        public void Restart()
        {
            Init();
        }

        [ContextMenu("重新开始(让电脑先手)")]
        public void SetAIFirst()
        {
            Init();
            allPiece[7, 7].image.sprite = Black;
            lazis[7, 7] = -1;
            AISprite = Black;
            PlaySprite = White;
        }
    }
}

