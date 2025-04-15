using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using NAudio.Wave;

namespace Galaxian
{
    public partial class Form1 : Form
    {
        // ブロッククラス
        public class Block
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public Image Img;
            public bool Alive = true; // エイリアン用
            public bool Used = false; // 自機の弾丸用

            public Block(int x, int y, int width, int height, Image img)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Img = img;
            }
        }

        // ゲームボード設定
        private const int TileSize = 32;
        private const int Rows = 16;
        private const int Columns = 16;
        private const int BoardWidth = TileSize * Columns;
        private const int BoardHeight = TileSize * Rows;

        // 自機
        private int ShipWidth = TileSize * 2;
        private int ShipHeight = TileSize;
        private int ShipX = TileSize * Columns / 2 - TileSize;
        private int ShipY = BoardHeight - TileSize * 2;
        private int ShipVelocityX = TileSize;
        private Block Ship;

        //旗艦
        private List<Block> FragshipArray = new List<Block>();
        private int FragshipWidth = TileSize * 2;
        private int FragshipHeight = TileSize;
        private int FragshipX = TileSize;
        private int FragshipY = TileSize;
        private int FragshipVelocityX = 1;

        // 赤エイリアン
        private List<Block> AlienRedArray = new List<Block>();
        private int AlienRedWidth = TileSize * 2;
        private int AlienRedHeight = TileSize;
        private int AlienRedX = TileSize;
        private int AlienRedY = TileSize * 2;
        private int AlienRedVelocityX = 1;

        // ピンクエイリアン
        private List<Block> AlienPinkArray = new List<Block>();
        private int AlienPinkWidth = TileSize * 2;
        private int AlienPinkHeight = TileSize;
        private int AlienPinkX = TileSize;
        private int AlienPinkY = TileSize * 3;
        private int AlienPinkVelocityX = 1;

        // シアンエイリアン
        private List<Block> AlienCyanArray = new List<Block>();
        private int AlienCyanWidth = TileSize * 2;
        private int AlienCyanHeight = TileSize;
        private int AlienCyanX = TileSize;
        private int AlienCyanY = TileSize * 4;
        private int AlienCyanVelocityX = 1;

        // 自機の弾丸
        private List<Block> BulletArray = new List<Block>();
        private int BulletWidth = TileSize / 8;
        private int BulletHeight = TileSize / 2;
        private int BulletVelocityY = -15;

        // 敵の弾丸
        private List<Block> EnemyBulletArray = new List<Block>();
        private int EnemyBulletWidth = TileSize / 8;
        private int EnemyBulletHeight = TileSize / 2;
        private int EnemyBulletVelocityY = 8;

        private System.Windows.Forms.Timer GameLoop;
        private int Score = 0;
        private int Lives = 3;
        private int Rounds = 1;
        private int NextLifeScore = 10000; // スコアが10000に達すると1UP
        private bool GameOver = false;

        private Image ShipImg;
        private Image FragshipImg;
        private Image AlienRedImg;
        private Image AlienPinkImg;
        private Image AlienCyanImg;
        private Sound GameStart;
        private Sound BackgroundSound;
        private SoundEffect ShootSound;
        private SoundEffect ExplosionSound;
        private SoundEffect BossExplosionSound;
        private SoundEffect Loss;
        private SoundEffect ExtraLife;

        private Random rand = new Random();

        public Form1()
        {
            // フォーム設定
            this.ClientSize = new Size(BoardWidth, BoardHeight);
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;

            // ウィンドウテキストを設定
            this.Text = "GALAXIAN";

            // 最大化を無効にする
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // 画像を読み込む
            ShipImg = Image.FromFile("Resources/Galacship.png");
            FragshipImg = Image.FromFile("Resources/Fragship.png");
            AlienRedImg = Image.FromFile("Resources/RedAlien.jpg");
            AlienPinkImg = Image.FromFile("Resources/PinkAlien.jpg");
            AlienCyanImg = Image.FromFile("Resources/CyanAlien.jpg");

            // サウンドを読み込む
            GameStart = new Sound("Resources/StartGame.wav");
            BackgroundSound = new Sound("Resources/backgroundMusic.wav");
            ShootSound = new SoundEffect("Resources/Shoot.wav");
            ExplosionSound = new SoundEffect("Resources/HitEnemy.wav");
            BossExplosionSound = new SoundEffect("Resources/HitBoss.wav");
            Loss = new SoundEffect("Resources/FighterLoss.wav");
            ExtraLife = new SoundEffect("Resources/Extra-Life.wav");

            // 自機を初期化
            Ship = new Block(ShipX, ShipY, ShipWidth, ShipHeight, ShipImg);

            // エイリアンを生成
            CreateFragship();
            CreateRedAliens();
            CreatePinkAliens();
            CreateCyanAliens();

            // タイマー設定
            GameLoop = new System.Windows.Forms.Timer();
            GameLoop.Interval = 1000 / 60; // 60FPS
            GameLoop.Tick += (sender, e) => { Move(); this.Invalidate(); };
            GameLoop.Start();
            GameStart.Play();
            BackgroundSound.Loop();

            // キーボードイベント
            this.KeyDown += OnKeyDown;
        }

        private void CreateFragship()
        {
            for (int r = 0; r < 1; r++) // 1行
            {
                for (int c = 0; c < 5; c++) // 5列
                {
                    FragshipArray.Add(new Block(
                        FragshipX + c * FragshipWidth,
                        FragshipY + r * FragshipHeight,
                        FragshipWidth,
                        FragshipHeight,
                        FragshipImg
                    ));
                }
            }
        }

        private void CreateRedAliens()
        {
            for (int r = 0; r < 1; r++) // 1行
            {
                for (int c = 0; c < 5; c++) // 5列
                {
                    AlienRedArray.Add(new Block(
                        AlienRedX + c * AlienRedWidth,
                        AlienRedY + r * AlienRedHeight,
                        AlienRedWidth,
                        AlienRedHeight,
                        AlienRedImg
                    ));
                }
            }
        }

        private void CreatePinkAliens()
        {
            for (int r = 0; r < 1; r++) // 1行
            {
                for (int c = 0; c < 5; c++) // 5列
                {
                    AlienPinkArray.Add(new Block(
                        AlienPinkX + c * AlienPinkWidth,
                        AlienPinkY + r * AlienPinkHeight,
                        AlienPinkWidth,
                        AlienPinkHeight,
                        AlienPinkImg
                    ));
                }
            }
        }

        private void CreateCyanAliens()
        {
            for (int r = 0; r < 1; r++) // 1行
            {
                for (int c = 0; c < 5; c++) // 5列
                {
                    AlienCyanArray.Add(new Block(
                        AlienCyanX + c * AlienCyanWidth,
                        AlienCyanY + r * AlienCyanHeight,
                        AlienCyanWidth,
                        AlienCyanHeight,
                        AlienCyanImg
                    ));
                }
            }
        }

        private void Move()
        {
            // 自機の弾丸移動
            for (int i = BulletArray.Count - 1; i >= 0; i--)
            {
                var bullet = BulletArray[i];
                bullet.Y += BulletVelocityY;

                // 弾丸と旗艦の当たり判定
                foreach (var alien in FragshipArray)
                {
                    if (alien.Alive && DetectCollision(bullet, alien))
                    {
                        alien.Alive = false;
                        bullet.Used = true;
                        Score += 60;
                        BossExplosionSound.Play();
                        CheckExtraLife();
                    }
                }

                // 弾丸と赤エイリアンの当たり判定
                foreach (var alien in AlienRedArray)
                {
                    if (alien.Alive && DetectCollision(bullet, alien))
                    {
                        alien.Alive = false;
                        bullet.Used = true;
                        Score += 50;
                        ExplosionSound.Play();
                        CheckExtraLife();
                    }
                }

                // 弾丸とピンクエイリアンの当たり判定
                foreach (var alien in AlienPinkArray)
                {
                    if (alien.Alive && DetectCollision(bullet, alien))
                    {
                        alien.Alive = false;
                        bullet.Used = true;
                        Score += 40;
                        ExplosionSound.Play();
                        CheckExtraLife();
                    }
                }

                // 弾丸とシアンエイリアンの当たり判定
                foreach (var alien in AlienCyanArray)
                {
                    if (alien.Alive && DetectCollision(bullet, alien))
                    {
                        alien.Alive = false;
                        bullet.Used = true;
                        Score += 30;
                        ExplosionSound.Play();
                        CheckExtraLife();
                    }
                }

                // 弾丸が画面外に出たら削除
                if (bullet.Y < 0 || bullet.Used)
                {
                    BulletArray.RemoveAt(i);
                }
            }

            //旗艦の移動
            foreach (var alien in FragshipArray)
            {
                if (alien.Alive)
                {
                    alien.X += FragshipVelocityX;
                    if (alien.X + alien.Width >= BoardWidth || alien.X <= 0)
                    {
                        FragshipVelocityX *= -1;
                        alien.X += FragshipVelocityX * 2;
                    }
                }
            }

            // 赤エイリアンの移動
            foreach (var alien in AlienRedArray)
            {
                if (alien.Alive)
                {
                    alien.X += AlienRedVelocityX;
                    if (alien.X + alien.Width >= BoardWidth || alien.X <= 0)
                    {
                        AlienRedVelocityX *= -1;
                        alien.X += AlienRedVelocityX * 2;
                    }
                }
            }

            // ピンクエイリアンの移動
            foreach (var alien in AlienPinkArray)
            {
                if (alien.Alive)
                {
                    alien.X += AlienPinkVelocityX;
                    if (alien.X + alien.Width >= BoardWidth || alien.X <= 0)
                    {
                        AlienPinkVelocityX *= -1;
                        alien.X += AlienPinkVelocityX * 2;
                    }
                }
            }

            // シアンエイリアンの移動
            foreach (var alien in AlienCyanArray)
            {
                if (alien.Alive)
                {
                    alien.X += AlienCyanVelocityX;
                    if (alien.X + alien.Width >= BoardWidth || alien.X <= 0)
                    {
                        AlienCyanVelocityX *= -1;
                        alien.X += AlienCyanVelocityX * 2;
                    }
                }
            }

            // 敵の弾丸の移動
            for (int i = 0; i < EnemyBulletArray.Count; i++)
            {
                Block EnemyBullet = EnemyBulletArray[i];
                EnemyBullet.Y += EnemyBulletVelocityY;

                // 敵の弾丸と自機の当たり判定
                if (DetectCollision(EnemyBullet, Ship))
                {
                    Lives--; // 残機-1
                    EnemyBulletArray.Clear(); // 敵弾丸を消す
                    BulletArray.Clear(); // 自機の弾丸を消す
                    Ship.X = ShipX; // 自機を初期位置に戻す
                    Loss.Play();
                    if (Lives == 0)
                    { // 残機が0になるとゲームオーバー
                        GameOver = true;
                        GameLoop.Stop();
                        BackgroundSound.Stop();
                        EnemyBulletArray.Clear();
                        BulletArray.Clear();
                        Ship.Y = -35; // 自機を画面外に出す
                        break;
                    }
                }
            }

            // 画面外に出た敵の弾丸を削除
            while (EnemyBulletArray.Count > 0 && EnemyBulletArray[0].Y > BoardHeight)
            {
                EnemyBulletArray.RemoveAt(0);
            }

            // 全ての敵が倒されたかを確認し、倒されていれば再生成
            // 弾丸をクリア
            if (AreAllEnemiesDefeated())
            {
                BulletArray.Clear();
                EnemyBulletArray.Clear();
                CreateFragship();
                CreateRedAliens();
                CreatePinkAliens();
                CreateCyanAliens();
                Rounds += 1;
            }

            // ランダムに敵が弾を撃つ
            FireEnemyBullets(FragshipArray);
            FireEnemyBullets(AlienRedArray);
            FireEnemyBullets(AlienPinkArray);
            FireEnemyBullets(AlienCyanArray);
        }

        private void CheckExtraLife()
        {
            if(Score >= NextLifeScore)
            {
                Lives++;
                ExtraLife.Play();
                NextLifeScore += 10000; // 次の1UPスコアを更新
            }
        }

        private void FireEnemyBullets(List<Block> enemies)
        {
            foreach (var alien in enemies)
            {
                if(alien.Alive && rand.Next(0, 200) == 0) // 1/200の確率
                {
                    EnemyBulletArray.Add(new Block(alien.X + alien.Width / 2 - EnemyBulletWidth / 2,
                                                   alien.Y + alien.Height, EnemyBulletWidth, EnemyBulletHeight, null));
                }
            }
        }

        private bool AreAllEnemiesDefeated()
        {
            // 各敵リストをチェックし、全ての敵が倒されている場合にtrueを返す
            return FragshipArray.All(alien => !alien.Alive) &&
                   AlienRedArray.All(alien => !alien.Alive) &&
                   AlienPinkArray.All(alien => !alien.Alive) &&
                   AlienCyanArray.All(alien => !alien.Alive);
        }

        private bool DetectCollision(Block a, Block b)
        {
            return a.X < b.X + b.Width &&
                   a.X + a.Width > b.X &&
                   a.Y < b.Y + b.Height &&
                   a.Y + a.Height > b.Y;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (GameOver)
            {
                // ゲームオーバー後のリセット処理
                ResetGame();
            }
            else
            {
                if (e.KeyCode == Keys.Left && Ship.X - ShipVelocityX >= 0)
                {
                    Ship.X -= ShipVelocityX;
                }
                else if (e.KeyCode == Keys.Right && Ship.X + Ship.Width + ShipVelocityX <= BoardWidth)
                {
                    Ship.X += ShipVelocityX;
                }
                else if (e.KeyCode == Keys.Space)
                {
                    BulletArray.Add(new Block(
                        Ship.X + Ship.Width / 2 - BulletWidth / 2,
                        Ship.Y,
                        BulletWidth,
                        BulletHeight,
                        null
                    ));
                    ShootSound.Play();
                }
            }
        }

        private void ResetGame()
        {
            // リセット処理
            GameOver = false;
            GameLoop.Start();
            Ship.X = ShipX;
            Ship.Y = ShipY;
            Score = 0;
            Lives = 3;
            Rounds = 1;
            FragshipArray.Clear();
            AlienRedArray.Clear();
            AlienPinkArray.Clear();
            AlienCyanArray.Clear();
            BulletArray.Clear();
            EnemyBulletArray.Clear();
            CreateFragship();
            CreateRedAliens();
            CreatePinkAliens();
            CreateCyanAliens();
            BackgroundSound.Loop();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // 自機描画
            g.DrawImage(Ship.Img, Ship.X, Ship.Y, Ship.Width, Ship.Height);

            // 旗艦描画
            foreach (var alien in FragshipArray)
            {
                if (alien.Alive)
                {
                    g.DrawImage(alien.Img, alien.X, alien.Y, alien.Width, alien.Height);
                }
            }

            // 赤エイリアン描画
            foreach (var alien in AlienRedArray)
            {
                if (alien.Alive)
                {
                    g.DrawImage(alien.Img, alien.X, alien.Y, alien.Width, alien.Height);
                }
            }

            // ピンクエイリアン描画
            foreach (var alien in AlienPinkArray)
            {
                if (alien.Alive)
                {
                    g.DrawImage(alien.Img, alien.X, alien.Y, alien.Width, alien.Height);
                }
            }

            // シアンエイリアン描画
            foreach (var alien in AlienCyanArray)
            {
                if (alien.Alive)
                {
                    g.DrawImage(alien.Img, alien.X, alien.Y, alien.Width, alien.Height);
                }
            }

            // 弾丸描画
            foreach (var bullet in BulletArray)
            {
                g.FillRectangle(Brushes.Yellow, bullet.X, bullet.Y, bullet.Width, bullet.Height);
            }

            // 敵の弾丸描画
            foreach (var bullet in EnemyBulletArray)
            {
                g.FillRectangle(Brushes.White, bullet.X, bullet.Y, bullet.Width, bullet.Height);
            }

            // スコア表示
            g.DrawString($"SCORE: {Score}", new Font("Arial", 16), Brushes.White, 10, 10);
            g.DrawString($"LIVES: {Lives}", new Font("Arial", 16), Brushes.White, 10, 475);
            g.DrawString($"ROUND {Rounds}", new Font("Arial", 16), Brushes.White, 365, 475);

            // ゲームオーバー
            if (GameOver)
            {
                g.DrawString("GAME OVER", new Font("Arial", 24), Brushes.Red, BoardWidth / 2 - 100, BoardHeight / 2);
            }
        }
    }
}