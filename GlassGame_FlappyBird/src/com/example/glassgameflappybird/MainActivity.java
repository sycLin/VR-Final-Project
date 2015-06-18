package com.example.glassgameflappybird;

import java.util.ArrayList;
import java.util.List;

import org.andengine.engine.camera.Camera;
import org.andengine.engine.handler.IUpdateHandler;
import org.andengine.engine.options.EngineOptions;
import org.andengine.engine.options.ScreenOrientation;
import org.andengine.engine.options.resolutionpolicy.RatioResolutionPolicy;
import org.andengine.entity.IEntity;
import org.andengine.entity.modifier.AlphaModifier;
import org.andengine.entity.modifier.IEntityModifier.IEntityModifierListener;
import org.andengine.entity.modifier.LoopEntityModifier;
import org.andengine.entity.modifier.MoveModifier;
import org.andengine.entity.modifier.ParallelEntityModifier;
import org.andengine.entity.modifier.SequenceEntityModifier;
import org.andengine.entity.scene.Scene;
import org.andengine.entity.scene.background.Background;
import org.andengine.entity.sprite.AnimatedSprite;
import org.andengine.entity.sprite.Sprite;
import org.andengine.entity.text.Text;
import org.andengine.entity.text.TextOptions;
import org.andengine.entity.util.FPSLogger;
import org.andengine.opengl.font.StrokeFont;
import org.andengine.opengl.texture.ITexture;
import org.andengine.opengl.texture.TextureOptions;
import org.andengine.opengl.texture.atlas.bitmap.BitmapTextureAtlas;
import org.andengine.opengl.texture.atlas.bitmap.BitmapTextureAtlasTextureRegionFactory;
import org.andengine.opengl.texture.atlas.bitmap.BuildableBitmapTextureAtlas;
import org.andengine.opengl.texture.atlas.bitmap.source.IBitmapTextureAtlasSource;
import org.andengine.opengl.texture.atlas.buildable.builder.BlackPawnTextureAtlasBuilder;
import org.andengine.opengl.texture.atlas.buildable.builder.ITextureAtlasBuilder.TextureAtlasBuilderException;
import org.andengine.opengl.texture.region.TextureRegion;
import org.andengine.opengl.texture.region.TiledTextureRegion;
import org.andengine.ui.activity.SimpleBaseGameActivity;
import org.andengine.util.HorizontalAlign;
import org.andengine.util.debug.Debug;
import org.andengine.util.modifier.IModifier;
import org.andengine.util.modifier.ease.EaseSineInOut;

import android.graphics.Color;
import android.graphics.Typeface;
import android.os.Bundle;
import android.view.MotionEvent;

import com.google.android.glass.touchpad.Gesture;
import com.google.android.glass.touchpad.GestureDetector;


public class MainActivity extends SimpleBaseGameActivity implements IUpdateHandler, GestureDetector.ScrollListener, GestureDetector.TwoFingerScrollListener,GestureDetector.BaseListener, GestureDetector.FingerListener{


	private static final int CAMERA_WIDTH = 640;
	private static final int CAMERA_HEIGHT = 360;
	private static final int INIT_X = 200;
	private static final int INIT_Y = 130;
	private static int numOfGround = 19;
	private static float Speed = 300;
	private static float JumpDelay = 0.4f;
	private static float JumpAngle = -30;
	private static float JumpRotateSpeed = 360;
	private static float Gravity = 1600;
	private static float JumpV = -450;
	private static int TubeCount = 4;
	private static float TubeDistance = 300;
	private static float TubeBase = 1000;
	private static float TubeDifRange = 170;
	
	//loaded texture
	private BuildableBitmapTextureAtlas mBitmapTextureAtlas;
	private TiledTextureRegion mPlayerTextureRegion;
	private TextureRegion mBackgroundTextureRegion;
	private TextureRegion mGroundTextureRegion;
	private TextureRegion mTubeTextureRegion;
	private TextureRegion mRestartTextureRegion;
	private TextureRegion mLeaderBoardTextureRegion;
	private ITexture fontTexture;
	private StrokeFont mStrokeFont;
	
	
	private GestureDetector mGestureDetector;
	
	
	//sprite instances
	private AnimatedSprite player;
	private Sprite background;
	private List<Sprite> grounds;
	private List<Sprite> tubes;
	private Sprite leaderBoard;
	private Sprite restart;
	private Text scoreText;
	
	//private Text endScoreText;
	//private Text endHighText;
	
	//animation
	private LoopEntityModifier startAnimation;
	private ParallelEntityModifier FadeIn;
	private ParallelEntityModifier FadeOut;
	private ParallelEntityModifier FadeIn2;
	private ParallelEntityModifier FadeOut2;
	
	
	//parameter
	private float JumpDelayCounter;
	private float Vy = 0;
	private float CurrentDistance;
	private int NowScore;
	private int HighScore;;
	
	

	
	//States
	private enum GameState
	{
		waiting,
		gaming,
		dyingAnimation,
		dying,
	};
	
	private GameState gameState = GameState.waiting;


		@Override
	  protected void onCreate(Bundle savedInstanceState) {
	        super.onCreate(savedInstanceState);
	        mGestureDetector = new GestureDetector(this)
	                .setScrollListener(this).setTwoFingerScrollListener(this).setBaseListener(this).setFingerListener(this);
	    }
	
	@Override
	public EngineOptions onCreateEngineOptions() {
		final Camera camera = new Camera(0, 0, CAMERA_WIDTH, CAMERA_HEIGHT);
		
		return new EngineOptions(true, ScreenOrientation.LANDSCAPE_FIXED, new RatioResolutionPolicy(CAMERA_WIDTH, CAMERA_HEIGHT), camera);
	}

	@Override
	public void onCreateResources() {
		BitmapTextureAtlasTextureRegionFactory.setAssetBasePath("gfx/");

		this.mBitmapTextureAtlas = new BuildableBitmapTextureAtlas(this.getTextureManager(), 2048, 1024, TextureOptions.NEAREST);
		this.mPlayerTextureRegion = BitmapTextureAtlasTextureRegionFactory.createTiledFromAsset(mBitmapTextureAtlas,this,"flappyBird/flappybird.png",3,1);
		this.mBackgroundTextureRegion= BitmapTextureAtlasTextureRegionFactory.createFromAsset(mBitmapTextureAtlas, this, "flappyBird/background.png");
		this.mGroundTextureRegion =  BitmapTextureAtlasTextureRegionFactory.createFromAsset(mBitmapTextureAtlas, this, "flappyBird/ground.png");
		this.mTubeTextureRegion = BitmapTextureAtlasTextureRegionFactory.createFromAsset(mBitmapTextureAtlas, this, "flappyBird/tube.png");
		this.mRestartTextureRegion = BitmapTextureAtlasTextureRegionFactory.createFromAsset(mBitmapTextureAtlas, this, "flappyBird/restart.png");
		this.mLeaderBoardTextureRegion = BitmapTextureAtlasTextureRegionFactory.createFromAsset(mBitmapTextureAtlas, this, "flappyBird/board.png");
		

		try {
			this.mBitmapTextureAtlas.build(new BlackPawnTextureAtlasBuilder<IBitmapTextureAtlasSource, BitmapTextureAtlas>(0, 0, 1));
			this.mBitmapTextureAtlas.load();
		} catch (TextureAtlasBuilderException e) {
			Debug.e(e);
		}
		
		fontTexture = new BitmapTextureAtlas(this.getTextureManager(), 256, 256, TextureOptions.BILINEAR);
		
		this.mStrokeFont = new StrokeFont(this.getFontManager(), this.fontTexture, Typeface.create(Typeface.DEFAULT, Typeface.BOLD), 64, true, Color.WHITE, 2, Color.BLACK);
		this.mStrokeFont.load();
		
		
	}
	

	private void gameInit()
	{
		Vy = 0;
		NowScore = 0;
		this.scoreText.setText("0");
		this.scoreText.setX(580-scoreText.getWidth()+32);
		
		
		this.player.setRotation(0);
		this.player.setPosition(INIT_X,INIT_Y);
		this.player.registerEntityModifier(this.startAnimation);
		this.player.animate(100);
		CurrentDistance = 400;
		 
		
		for(int i=0;i<TubeCount;i++)
		{
			Sprite tube1 = tubes.get(i*2);
			Sprite tube2 = tubes.get(i*2+1);
		
			float dif = (float)(Math.random() * TubeDifRange - TubeDifRange/2);
		
		
			tube1.setY(0 + dif);
			tube2.setY(-500 + dif);
		
			tube1.setX(TubeBase + i*TubeDistance);
			tube2.setX(TubeBase + i*TubeDistance);
		}
	}
	
	@Override
	public Scene onCreateScene() {
		this.mEngine.registerUpdateHandler(new FPSLogger());

		final Scene scene = new Scene();
		scene.setBackground(new Background(0.09804f, 0.6274f, 0.8784f));
		
		//background
		this.background = new Sprite(0,0,this.mBackgroundTextureRegion,this.getVertexBufferObjectManager());
		float backgroundY =CAMERA_HEIGHT - this.background.getHeight() - 50;
		this.background.setY(backgroundY);
		scene.attachChild(background);
		
		

		
		//PathModifier pModifier = new PositionModifer();
		
		//FlappyBird Animation
		MoveModifier move1 = new MoveModifier(0.5f,INIT_X,INIT_X,INIT_Y,INIT_Y+20,EaseSineInOut.getInstance());
		MoveModifier move2 = new MoveModifier(0.5f,INIT_X,INIT_X,INIT_Y+20,INIT_Y,EaseSineInOut.getInstance());
		SequenceEntityModifier sequence = new SequenceEntityModifier(move1,move2);
		this.startAnimation = new LoopEntityModifier(sequence);
		
		
		this.mEngine.registerUpdateHandler(this);
		

		tubes = new ArrayList<Sprite>();
		for(int i = 0; i<TubeCount;i++)
		{
			Sprite tube1 = new Sprite(0,0,this.mTubeTextureRegion,this.getVertexBufferObjectManager());
			Sprite tube2 = new Sprite(0,0,this.mTubeTextureRegion,this.getVertexBufferObjectManager());
			
			tube1.setScale(0.5f);
			tube2.setScale(0.5f);
			
			tube2.setRotation(180);
			
			tubes.add(tube1);
			tubes.add(tube2);
			
			scene.attachChild(tube1);
			scene.attachChild(tube2);
		}
		
		this.player = new AnimatedSprite(0,0,this.mPlayerTextureRegion,this.getVertexBufferObjectManager());

		this.player.setRotationCenter(46, 32);
		this.player.setScale(0.6f);
		scene.attachChild(this.player);
	
		
		grounds = new ArrayList<Sprite>();
		for(int i =0;i<numOfGround;i++)
		{
			Sprite ground = new Sprite(i*37,300,this.mGroundTextureRegion,this.getVertexBufferObjectManager());
			ground.setScaleX(1.01f);
			scene.attachChild(ground);
			
			grounds.add(ground);
		}
		
		
		this.restart = new Sprite(0,0,this.mRestartTextureRegion,this.getVertexBufferObjectManager());
		this.leaderBoard = new Sprite(0,0,this.mLeaderBoardTextureRegion,this.getVertexBufferObjectManager());
		this.restart.setScale(0.6f);
		this.leaderBoard.setScale(0.6f);
		this.restart.setAlpha(0);
		this.leaderBoard.setAlpha(0);
		
		scene.attachChild(this.leaderBoard);
		scene.attachChild(this.restart);
		

		scoreText = new Text(580, 10, this.mStrokeFont, "0123456789", new TextOptions(HorizontalAlign.RIGHT), this.getVertexBufferObjectManager());
		//this.scoreText.setX(640-scoreText.getWidth());
		//scoreText.setColor(pRed, pGreen, pBlue);
		scene.attachChild(this.scoreText);
		
		gameInit();
		
		this.FadeIn = new ParallelEntityModifier(new IEntityModifierListener(){

			@Override
			public void onModifierStarted(IModifier<IEntity> pModifier,
					IEntity pItem) {
				// TODO Auto-generated method stub
				
			}

			@Override
			public void onModifierFinished(IModifier<IEntity> pModifier,
					IEntity pItem) {
				
				gameState = GameState.dying;
				
			}},
				new MoveModifier(0.3f,210,210,180,200),
				new AlphaModifier(0.3f,0,1f)
				);
		
		
		this.FadeOut = new ParallelEntityModifier(
				new MoveModifier(0.1f,210,210,200,180),
				new AlphaModifier(0.1f,1,0f)
				);
		

		this.FadeIn2 = new ParallelEntityModifier(
				new MoveModifier(0.3f,230,230,0,10),
				new AlphaModifier(0.3f,0,1f)
				);
		
		
		this.FadeOut2 = new ParallelEntityModifier(
				new MoveModifier(0.1f,230,230,10,0),
				new AlphaModifier(0.1f,1,0f)
				);
		
		
		
		return scene;
	}
	
	private void updateTubes(float pSeconds)
	{
		for(int i=0;i<TubeCount;i++)
		{
			Sprite tube1 = tubes.get(i*2);
			Sprite tube2 = tubes.get(i*2+1);
		
			
			float tubeX = tube1.getX();
			
			tubeX -= pSeconds*Speed;
			
			if(tubeX<-200)
			{

				float dif = (float)(Math.random() * TubeDifRange - TubeDifRange/2);
			
				tube1.setY(0 + dif);
				tube2.setY(-500 + dif);
				
				tube1.setX(tubeX+TubeCount*TubeDistance);
				tube2.setX(tubeX+TubeCount*TubeDistance);
				
				
			}
			else
			{
				tube1.setX(tubeX);
				tube2.setX(tubeX);
			}

		}
	}
	
	//IUpdateHandler
	@Override
	public void onUpdate(float pSecondsElapsed) {
		// TODO Auto-generated method stub
		
		
		if(gameState == GameState.waiting)
		{
			updateGround(pSecondsElapsed);
		}
		else if(gameState == GameState.gaming)
		{
			CurrentDistance+= pSecondsElapsed*Speed;
			
			if(CurrentDistance>TubeBase)
			{
				Integer score = (int)((CurrentDistance - TubeBase)/TubeDistance);
				NowScore = score;
				if(NowScore>HighScore)
				{
					HighScore = NowScore;
				}
				
				this.scoreText.setText(score.toString());
				this.scoreText.setX(580-scoreText.getWidth()+32);
			}
			
			
			updateGround(pSecondsElapsed);
			updateTubes(pSecondsElapsed);
			
			
			if(JumpDelayCounter>0)
			{
				JumpDelayCounter -=pSecondsElapsed;
			}
			else
			{
				float r = player.getRotation();
				
				if(r<90)
				{
					r += JumpRotateSpeed * pSecondsElapsed;
					player.setRotation(r);
				}
				else
				{
					r = 90;
					player.setRotation(r);
				}
				
			}
			
			
			Vy += Gravity*pSecondsElapsed;
			
			float playerY = player.getY();
			playerY += Vy*pSecondsElapsed;
			
			if(playerY<-30)
			{
				playerY=-30;
				Vy =0;
			}
			

			if(playerY>265)
			{
				playerY = 265;
				kill();
			
			}
			else if(IsCollideAnyTube(10))
			{
				kill();
			}
				
				player.setY(playerY);
			
		}
		else if(gameState == GameState.dyingAnimation)
		{
			
		}
		else if(gameState == GameState.dying)
		{
			
		}
		//this.player.setRotation(this.player.getRotation()+pSecondsElapsed*360);
		
	}
	
	private void kill()
	{
		gameState = GameState.dyingAnimation;
		player.stopAnimation();
		this.restart.clearEntityModifiers();
		this.FadeIn.reset();
		this.restart.registerEntityModifier(this.FadeIn);
		
		this.leaderBoard.clearEntityModifiers();
		this.FadeIn2.reset();
		this.leaderBoard.registerEntityModifier(this.FadeIn2);
		
	}
	
	private boolean IsCollideAnyTube(float padding)
	{
		for(int i=0;i<tubes.size();i++)
		{
			if(IsCollide(player,tubes.get(i),padding))
			{
				return true;
			}
		}
		
		return false;
	}
	
	private boolean IsCollide(Sprite s1,Sprite s2,float padding)
	{
		//s1.collidesWith(pOtherShape)
		//return s1.collidesWith(s2);
		
		float s1CenterX = s1.getX() + (s1.getWidth()/2);
		float s1CenterY = s1.getY() + (s1.getHeight()/2);
		float s1leftX = s1CenterX - s1.getWidthScaled()/2 + padding;
		float s1rightX = s1CenterX + s1.getWidthScaled()/2 - padding;
		float s1topY = s1CenterY - s1.getHeightScaled()/2 + padding;
		float s1downY = s1CenterY + s1.getHeightScaled()/2 - padding;
		
		
		float s2CenterX = s2.getX() + (s2.getWidth()/2);
		float s2CenterY = s2.getY() + (s2.getHeight()/2);
		float s2leftX = s2CenterX - s2.getWidthScaled()/2;
		float s2rightX = s2CenterX + s2.getWidthScaled()/2;
		float s2topY = s2CenterY - s2.getHeightScaled()/2;
		float s2downY = s2CenterY + s2.getHeightScaled()/2;
		
		if(s1leftX < s2rightX && s1rightX > s2leftX && s1topY < s2downY && s1downY>s2topY)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private void updateGround(float pSecondsElapsed)
	{
		float dif = pSecondsElapsed * Speed;
		
		for(int i=0;i<numOfGround;i++)
		{
			Sprite ground = grounds.get(i);
			
			float x = ground.getX();
			x -= dif;
			
			if(x<-40)
			{
				x+=numOfGround*ground.getWidth();
			}
			
			 ground.setX(x);
			
		}
	}

	//IUpdateHandler
	@Override
	public void reset() {
		// TODO Auto-generated method stub
		
	}

	
	  @Override
	    public boolean onGenericMotionEvent(MotionEvent event) {
	        return mGestureDetector.onMotionEvent(event);
	    }

	    @Override
	    public boolean onScroll(float displacement, float delta, float velocity) {
	       
	        updateScrollInfo(displacement, delta, velocity);
	        return false;
	    }

	    @Override
	    public boolean onTwoFingerScroll(float displacement, float delta, float velocity) {

	    	
	        updateScrollInfo(displacement, delta, velocity);
	        return false;
	    }

	    /**
	     * Updates the text views that show the detailed scroll information.
	     *
	     * @param displacement the scroll displacement (position relative to the original touch-down
	     *     event)
	     * @param delta the scroll delta from the previous touch event
	     * @param velocity the velocity of the scroll event
	     */
	    private void updateScrollInfo(float displacement, float delta, float velocity) {

	    		
	    }
	// ===========================================================
	// Methods
	// ===========================================================

		@Override
		public void onFingerCountChanged(int previous, int current) {
			// TODO Auto-generated method stub
			
			
			if(current>previous)
			{
				tap();
			}
			
		}
		
		private void tap()
		{
			if(gameState == GameState.waiting)
			{
				player.clearEntityModifiers();
				gameState = GameState.gaming;
				jump();
			}
			else if(gameState == GameState.gaming)
			{
				jump();
				
			}
			else if(gameState == GameState.dyingAnimation)
			{
				
			}
			else if(gameState == GameState.dying)
			{
				gameInit();
				gameState = GameState.waiting;
				
				this.restart.clearEntityModifiers();
				this.FadeOut.reset();
				this.restart.registerEntityModifier(this.FadeOut);
				
				this.leaderBoard.clearEntityModifiers();
				this.FadeOut2.reset();
				this.leaderBoard.registerEntityModifier(FadeOut2);
			}
			
		}
		
		
		private void jump()
		{
			JumpDelayCounter = JumpDelay;
			Vy = JumpV;
			
			player.setRotation(JumpAngle);
		}

		@Override
		public boolean onGesture(Gesture gesture) {
			// TODO Auto-generated method stub
			if (gesture == Gesture.SWIPE_DOWN)
			{
				
			}
			return false;
		}

	// ===========================================================
	// Inner and Anonymous Classes
	// ===========================================================
}
