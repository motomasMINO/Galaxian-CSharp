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
        // �u���b�N�N���X
        public class Block
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public Image Img;
            public bool Alive = true; // �G�C���A���p
            public bool Used = false; // ���@�̒e�ۗp

            public Block(int x, int y, int width, int height, Image img)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Img = img;
            }
        }

        // �Q�[���{�[�h�ݒ�
        private const int TileSize = 32;
        private const int Rows = 16;
        private const int Columns = 16;
        private const int BoardWidth = TileSize * Columns;
        private const int BoardHeight = TileSize * Rows;

        // ���@
        private int ShipWidth = TileSize * 2;
        private int ShipHeight = TileSize;
        private int ShipX = TileSize * Columns / 2 - TileSize;
        private int ShipY = BoardHeight - TileSize * 2;
        private int ShipVelocityX = TileSize;
        private Block Ship;

        //����
        private List<Block> FragshipArray = new List<Block>();
        private int FragshipWidth = TileSize * 2;
        private int FragshipHeight = TileSize;
        private int FragshipX = TileSize;
        private int FragshipY = TileSize;
        private int FragshipVelocityX = 1;

        // �ԃG�C���A��
        private List<Block> AlienRedArray = new List<Block>();
        private int AlienRedWidth = TileSize * 2;
        private int AlienRedHeight = TileSize;
        private int AlienRedX = TileSize;
        private int AlienRedY = TileSize * 2;
        private int AlienRedVelocityX = 1;

        // �s���N�G�C���A��
        private List<Block> AlienPinkArray = new List<Block>();
        private int AlienPinkWidth = TileSize * 2;
        private int AlienPinkHeight = TileSize;
        private int AlienPinkX = TileSize;
        private int AlienPinkY = TileSize * 3;
        private int AlienPinkVelocityX = 1;

        // �V�A���G�C���A��
        private List<Block> AlienCyanArray = new List<Block>();
        private int AlienCyanWidth = TileSize * 2;
        private int AlienCyanHeight = TileSize;
        private int AlienCyanX = TileSize;
        private int AlienCyanY = TileSize * 4;
        private int AlienCyanVelocityX = 1;

        // ���@�̒e��
        private List<Block> BulletArray = new List<Block>();
        private int BulletWidth = TileSize / 8;
        private int BulletHeight = TileSize / 2;
        private int BulletVelocityY = -15;

        // �G�̒e��
        private List<Block> EnemyBulletArray = new List<Block>();
        private int EnemyBulletWidth = TileSize / 8;
        private int EnemyBulletHeight = TileSize / 2;
        private int EnemyBulletVelocityY = 8;

        private System.Windows.Forms.Timer GameLoop;
        private int Score = 0;
        private int Lives = 3;
        private int Rounds = 1;
        private int NextLifeScore = 10000; // �X�R�A��10000�ɒB�����1UP
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
            // �t�H�[���ݒ�
            this.ClientSize = new Size(BoardWidth, BoardHeight);
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;

            // �E�B���h�E�e�L�X�g��ݒ�
            this.Text = "GALAXIAN";

            // �ő剻�𖳌��ɂ���
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // �摜��ǂݍ���
            ShipImg = Image.FromFile("Resources/Galacship.png");
            FragshipImg = Image.FromFile("Resources/Fragship.png");
            AlienRedImg = Image.FromFile("Resources/RedAlien.jpg");
            AlienPinkImg = Image.FromFile("Resources/PinkAlien.jpg");
            AlienCyanImg = Image.FromFile("Resources/CyanAlien.jpg");

            // �T�E���h��ǂݍ���
            GameStart = new Sound("Resources/StartGame.wav");
            BackgroundSound = new Sound("Resources/backgroundMusic.wav");
            ShootSound = new SoundEffect("Resources/Shoot.wav");
            ExplosionSound = new SoundEffect("Resources/HitEnemy.wav");
            BossExplosionSound = new SoundEffect("Resources/HitBoss.wav");
            Loss = new SoundEffect("Resources/FighterLoss.wav");
            ExtraLife = new SoundEffect("Resources/Extra-Life.wav");

            // ���@��������
            Ship = new Block(ShipX, ShipY, ShipWidth, ShipHeight, ShipImg);

            // �G�C���A���𐶐�
            CreateFragship();
            CreateRedAliens();
            CreatePinkAliens();
            CreateCyanAliens();

            // �^�C�}�[�ݒ�
            GameLoop = new System.Windows.Forms.Timer();
            GameLoop.Interval = 1000 / 60; // 60FPS
            GameLoop.Tick += (sender, e) => { Move(); this.Invalidate(); };
            GameLoop.Start();
            GameStart.Play();
            BackgroundSound.Loop();

            // �L�[�{�[�h�C�x���g
            this.KeyDown += OnKeyDown;
        }

        private void CreateFragship()
        {
            for (int r = 0; r < 1; r++) // 1�s
            {
                for (int c = 0; c < 5; c++) // 5��
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
            for (int r = 0; r < 1; r++) // 1�s
            {
                for (int c = 0; c < 5; c++) // 5��
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
            for (int r = 0; r < 1; r++) // 1�s
            {
                for (int c = 0; c < 5; c++) // 5��
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
            for (int r = 0; r < 1; r++) // 1�s
            {
                for (int c = 0; c < 5; c++) // 5��
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
            // ���@�̒e�ۈړ�
            for (int i = BulletArray.Count - 1; i >= 0; i--)
            {
                var bullet = BulletArray[i];
                bullet.Y += BulletVelocityY;

                // �e�ۂƊ��͂̓����蔻��
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

                // �e�ۂƐԃG�C���A���̓����蔻��
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

                // �e�ۂƃs���N�G�C���A���̓����蔻��
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

                // �e�ۂƃV�A���G�C���A���̓����蔻��
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

                // �e�ۂ���ʊO�ɏo����폜
                if (bullet.Y < 0 || bullet.Used)
                {
                    BulletArray.RemoveAt(i);
                }
            }

            //���͂̈ړ�
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

            // �ԃG�C���A���̈ړ�
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

            // �s���N�G�C���A���̈ړ�
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

            // �V�A���G�C���A���̈ړ�
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

            // �G�̒e�ۂ̈ړ�
            for (int i = 0; i < EnemyBulletArray.Count; i++)
            {
                Block EnemyBullet = EnemyBulletArray[i];
                EnemyBullet.Y += EnemyBulletVelocityY;

                // �G�̒e�ۂƎ��@�̓����蔻��
                if (DetectCollision(EnemyBullet, Ship))
                {
                    Lives--; // �c�@-1
                    EnemyBulletArray.Clear(); // �G�e�ۂ�����
                    BulletArray.Clear(); // ���@�̒e�ۂ�����
                    Ship.X = ShipX; // ���@�������ʒu�ɖ߂�
                    Loss.Play();
                    if (Lives == 0)
                    { // �c�@��0�ɂȂ�ƃQ�[���I�[�o�[
                        GameOver = true;
                        GameLoop.Stop();
                        BackgroundSound.Stop();
                        EnemyBulletArray.Clear();
                        BulletArray.Clear();
                        Ship.Y = -35; // ���@����ʊO�ɏo��
                        break;
                    }
                }
            }

            // ��ʊO�ɏo���G�̒e�ۂ��폜
            while (EnemyBulletArray.Count > 0 && EnemyBulletArray[0].Y > BoardHeight)
            {
                EnemyBulletArray.RemoveAt(0);
            }

            // �S�Ă̓G���|���ꂽ�����m�F���A�|����Ă���΍Đ���
            // �e�ۂ��N���A
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

            // �����_���ɓG���e������
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
                NextLifeScore += 10000; // ����1UP�X�R�A���X�V
            }
        }

        private void FireEnemyBullets(List<Block> enemies)
        {
            foreach (var alien in enemies)
            {
                if(alien.Alive && rand.Next(0, 200) == 0) // 1/200�̊m��
                {
                    EnemyBulletArray.Add(new Block(alien.X + alien.Width / 2 - EnemyBulletWidth / 2,
                                                   alien.Y + alien.Height, EnemyBulletWidth, EnemyBulletHeight, null));
                }
            }
        }

        private bool AreAllEnemiesDefeated()
        {
            // �e�G���X�g���`�F�b�N���A�S�Ă̓G���|����Ă���ꍇ��true��Ԃ�
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
                // �Q�[���I�[�o�[��̃��Z�b�g����
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
            // ���Z�b�g����
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

            // ���@�`��
            g.DrawImage(Ship.Img, Ship.X, Ship.Y, Ship.Width, Ship.Height);

            // ���͕`��
            foreach (var alien in FragshipArray)
            {
                if (alien.Alive)
                {
                    g.DrawImage(alien.Img, alien.X, alien.Y, alien.Width, alien.Height);
                }
            }

            // �ԃG�C���A���`��
            foreach (var alien in AlienRedArray)
            {
                if (alien.Alive)
                {
                    g.DrawImage(alien.Img, alien.X, alien.Y, alien.Width, alien.Height);
                }
            }

            // �s���N�G�C���A���`��
            foreach (var alien in AlienPinkArray)
            {
                if (alien.Alive)
                {
                    g.DrawImage(alien.Img, alien.X, alien.Y, alien.Width, alien.Height);
                }
            }

            // �V�A���G�C���A���`��
            foreach (var alien in AlienCyanArray)
            {
                if (alien.Alive)
                {
                    g.DrawImage(alien.Img, alien.X, alien.Y, alien.Width, alien.Height);
                }
            }

            // �e�ە`��
            foreach (var bullet in BulletArray)
            {
                g.FillRectangle(Brushes.Yellow, bullet.X, bullet.Y, bullet.Width, bullet.Height);
            }

            // �G�̒e�ە`��
            foreach (var bullet in EnemyBulletArray)
            {
                g.FillRectangle(Brushes.White, bullet.X, bullet.Y, bullet.Width, bullet.Height);
            }

            // �X�R�A�\��
            g.DrawString($"SCORE: {Score}", new Font("Arial", 16), Brushes.White, 10, 10);
            g.DrawString($"LIVES: {Lives}", new Font("Arial", 16), Brushes.White, 10, 475);
            g.DrawString($"ROUND {Rounds}", new Font("Arial", 16), Brushes.White, 365, 475);

            // �Q�[���I�[�o�[
            if (GameOver)
            {
                g.DrawString("GAME OVER", new Font("Arial", 24), Brushes.Red, BoardWidth / 2 - 100, BoardHeight / 2);
            }
        }
    }
}