package com.gardify.android.ui.generic.materialShowCase;

import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.PorterDuff;
import android.graphics.PorterDuffXfermode;
import android.os.Build;
import android.os.Handler;
import android.util.AttributeSet;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewTreeObserver;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.gardify.android.R;
import com.gardify.android.ui.generic.materialShowCase.animation.AnimationFactory;
import com.gardify.android.ui.generic.materialShowCase.animation.AnimationListener;
import com.gardify.android.ui.generic.materialShowCase.animation.MaterialIntroListener;
import com.gardify.android.ui.generic.materialShowCase.shape.Circle;
import com.gardify.android.ui.generic.materialShowCase.shape.Focus;
import com.gardify.android.ui.generic.materialShowCase.shape.FocusGravity;
import com.gardify.android.ui.generic.materialShowCase.shape.Rect;
import com.gardify.android.ui.generic.materialShowCase.shape.Shape;
import com.gardify.android.ui.generic.materialShowCase.shape.ShapeType;
import com.gardify.android.ui.generic.materialShowCase.target.Target;
import com.gardify.android.ui.generic.materialShowCase.target.ViewTarget;

import static com.gardify.android.utils.ImageUtils.dpToPx;


/**
 * Taken from mertsimsek github repository,
 * adapted and customized for personal use.
 */
public class MaterialShowCaseView extends RelativeLayout {

    private ButtonClickListener buttonClickListener;

    /**
     * Mask color
     */
    private int maskColor;

    /**
     * MaterialIntroView will start
     * showing after delayMillis seconds
     * passed
     */
    private long delayMillis;

    /**
     * We don't draw MaterialIntroView
     * until isReady field set to true
     */
    private boolean isReady;

    /**
     * Show/Dismiss MaterialIntroView
     * with fade in/out animation if
     * this is enabled.
     */
    private boolean isFadeAnimationEnabled;

    /**
     * Animation duration
     */
    private long fadeAnimationDuration;

    /**
     * targetShape focus on target
     * and clear circle to focus
     */
    private Shape targetShape;

    /**
     * Focus Type
     */
    private Focus focusType;

    /**
     * FocusGravity type
     */
    private FocusGravity focusGravity;

    /**
     * Target View
     */
    private Target targetView;

    /**
     * Eraser
     */
    private Paint eraser;

    /**
     * Handler will be used to
     * delay MaterialIntroView
     */
    private Handler handler;

    /**
     * All views will be drawn to
     * this bitmap and canvas then
     * bitmap will be drawn to canvas
     */
    private Bitmap bitmap;
    private Canvas canvas;

    /**
     * Circle padding
     */
    private int padding;

    /**
     * Layout width/height
     */
    private int width;
    private int height;

    /**
     * Dismiss on touch any position
     */
    private boolean dismissOnTouch;

    /**
     * Info dialog view
     */
    private View infoView;

    /**
     * total showCase Count
     */
    private TextView textViewTotalShowcaseCount;

    /**
     * Title Text
     */
    private TextView textViewTitle;

    /**
     * Info Dialog Text
     */
    private TextView textViewInfo;

    /**
     * Info dialog text color
     */
    private int colorTextViewInfo;

    /**
     * Info dialog will be shown
     * If this value true
     */
    private boolean isInfoEnabled;

    /**
     * Dot view will appear center of
     * cleared target area
     */
    private View dotView;

    /**
     * Dot View will be shown if
     * this is true
     */
    private boolean isDotViewEnabled;

    /**
     * Image View will be shown if
     * this is true
     */
    private boolean isImageViewEnabled;

    /**
     * When layout completed, we set this true
     * Otherwise onGlobalLayoutListener stuck on loop.
     */
    private boolean isLayoutCompleted;

    /**
     * Notify user when MaterialIntroView is dismissed
     */
    private MaterialIntroListener materialIntroListener;

    /**
     * Perform click operation to target
     * if this is true
     */
    private boolean isPerformClick;

    /**
     * Shape of target
     */
    private ShapeType shapeType;

    /**
     * Use custom shape
     */
    private boolean usesCustomShape = false;

    public MaterialShowCaseView(Context context) {
        super(context);
        init(context);
    }

    public MaterialShowCaseView(Context context, AttributeSet attrs) {
        super(context, attrs);
        init(context);
    }

    public MaterialShowCaseView(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
        init(context);
    }

    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    public MaterialShowCaseView(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) {
        super(context, attrs, defStyleAttr, defStyleRes);
        init(context);
    }

    private void init(Context context) {
        setWillNotDraw(false);
        setVisibility(INVISIBLE);

        /**
         * set default values
         */
        maskColor = Constants.DEFAULT_MASK_COLOR;
        delayMillis = Constants.DEFAULT_DELAY_MILLIS;
        fadeAnimationDuration = Constants.DEFAULT_FADE_DURATION;
        padding = Constants.DEFAULT_TARGET_PADDING;
        colorTextViewInfo = Constants.DEFAULT_COLOR_TEXTVIEW_INFO;
        focusType = Focus.ALL;
        focusGravity = FocusGravity.CENTER;
        shapeType = ShapeType.CIRCLE;
        isReady = false;
        isFadeAnimationEnabled = true;
        dismissOnTouch = false;
        isLayoutCompleted = false;
        isInfoEnabled = false;
        isDotViewEnabled = false;
        isPerformClick = false;
        isImageViewEnabled = true;

        /**
         * initialize objects
         */
        handler = new Handler();

        eraser = new Paint();
        eraser.setColor(0xFFFFFFFF);
        eraser.setXfermode(new PorterDuffXfermode(PorterDuff.Mode.CLEAR));
        eraser.setFlags(Paint.ANTI_ALIAS_FLAG);

        LayoutInflater layoutInflater = (LayoutInflater) getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        View layoutInfo = layoutInflater.inflate(R.layout.showcase_guided_tour, null);

        infoView = layoutInfo.findViewById(R.id.info_layout);
        ImageView closeBtnImage = layoutInfo.findViewById(R.id.imageView_home_popup_close);
        closeBtnImage.setOnClickListener(v -> {
            dismiss();
        });
        ImageView btnPrevious = layoutInfo.findViewById(R.id.imageView_home_previous);
        btnPrevious.setOnClickListener(v -> {
            buttonClickListener.onClick(false, true);
            dismiss();
        });
        ImageView btnNext = layoutInfo.findViewById(R.id.imageView_home_next);
        btnNext.setOnClickListener(v -> {
            buttonClickListener.onClick(true, false);
            dismiss();
        });
        textViewTitle = (TextView) layoutInfo.findViewById(R.id.textview_info_title);
        textViewTotalShowcaseCount = layoutInfo.findViewById(R.id.textView_total_showcase_count);
        textViewInfo = (TextView) layoutInfo.findViewById(R.id.textview_info);
        textViewInfo.setTextColor(colorTextViewInfo);
        dotView = LayoutInflater.from(getContext()).inflate(R.layout.showcase_dotview, null);
        dotView.measure(MeasureSpec.UNSPECIFIED, MeasureSpec.UNSPECIFIED);
        getViewTreeObserver().addOnGlobalLayoutListener(new ViewTreeObserver.OnGlobalLayoutListener() {
            @Override
            public void onGlobalLayout() {
                targetShape.reCalculateAll();
                if (targetShape != null && targetShape.getPoint().y != 0 && !isLayoutCompleted) {
                    if (isInfoEnabled)
                        setInfoLayout();
                    if (isDotViewEnabled)
                        setDotViewLayout();
                    removeOnGlobalLayoutListener(MaterialShowCaseView.this, this);
                }
            }
        });

    }

    @TargetApi(Build.VERSION_CODES.JELLY_BEAN)
    public static void removeOnGlobalLayoutListener(View v, ViewTreeObserver.OnGlobalLayoutListener listener) {
        if (Build.VERSION.SDK_INT < 16) {
            v.getViewTreeObserver().removeGlobalOnLayoutListener(listener);
        } else {
            v.getViewTreeObserver().removeOnGlobalLayoutListener(listener);
        }
    }

    @Override
    protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec) {
        super.onMeasure(widthMeasureSpec, heightMeasureSpec);

        width = getMeasuredWidth();
        height = getMeasuredHeight();
    }

    @Override
    protected void onDraw(Canvas canvas) {
        super.onDraw(canvas);

        if (!isReady) return;

        if (bitmap == null || canvas == null) {
            if (bitmap != null) bitmap.recycle();

            bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);
            this.canvas = new Canvas(bitmap);
        }

        /**
         * Draw mask
         */
        this.canvas.drawColor(Color.TRANSPARENT, PorterDuff.Mode.CLEAR);
        this.canvas.drawColor(maskColor);

        /**
         * Clear focus area
         */
        targetShape.draw(this.canvas, eraser, padding);

        canvas.drawBitmap(bitmap, 0, 0, null);
    }

    /**
     * Perform click operation when user
     * touches on target circle.
     *
     * @param event
     * @return
     */
    @Override
    public boolean onTouchEvent(MotionEvent event) {
        float xT = event.getX();
        float yT = event.getY();

        boolean isTouchOnFocus = targetShape.isTouchOnFocus(xT, yT);

        switch (event.getAction()) {
            case MotionEvent.ACTION_DOWN:

                if (isTouchOnFocus && isPerformClick) {
                    targetView.getView().setPressed(true);
                    targetView.getView().invalidate();
                }

                return true;
            case MotionEvent.ACTION_UP:

                if (isTouchOnFocus || dismissOnTouch)
                    dismiss();

                if (isTouchOnFocus && isPerformClick) {
                    targetView.getView().performClick();
                    targetView.getView().setPressed(true);
                    targetView.getView().invalidate();
                    targetView.getView().setPressed(false);
                    targetView.getView().invalidate();
                }

                return true;
            default:
                break;
        }

        return super.onTouchEvent(event);
    }

    /**
     * Shows material view with fade in
     * animation
     *
     * @param activity
     */
    private void show(Activity activity) {

        ((ViewGroup) activity.getWindow().getDecorView()).addView(this);

        setReady(true);

        handler.postDelayed(new Runnable() {
            @Override
            public void run() {
                if (isFadeAnimationEnabled)
                    AnimationFactory.animateFadeIn(MaterialShowCaseView.this, fadeAnimationDuration, new AnimationListener.OnAnimationStartListener() {
                        @Override
                        public void onAnimationStart() {
                            setVisibility(VISIBLE);
                        }
                    });
                else
                    setVisibility(VISIBLE);
            }
        }, delayMillis);

    }

    /**
     * Dismiss Material Intro View
     */
    public void dismiss() {

        AnimationFactory.animateFadeOut(this, fadeAnimationDuration, () -> {
            setVisibility(GONE);
            removeMaterialView();
        });
    }

    private void removeMaterialView() {
        if (getParent() != null)
            ((ViewGroup) getParent()).removeView(this);
    }

    /**
     * locate info card view above/below the
     * circle. If circle's Y coordiante is bigger than
     * Y coordinate of root view, then locate cardview
     * above the circle. Otherwise locate below.
     */
    private void setInfoLayout() {

        handler.post(new Runnable() {
            @Override
            public void run() {
                isLayoutCompleted = true;

                if (infoView.getParent() != null)
                    ((ViewGroup) infoView.getParent()).removeView(infoView);

                RelativeLayout.LayoutParams infoDialogParams = new RelativeLayout.LayoutParams(
                        ViewGroup.LayoutParams.MATCH_PARENT,
                        ViewGroup.LayoutParams.FILL_PARENT);

                if (targetShape.getPoint().y < height / 2) {
                    ((RelativeLayout) infoView).setGravity(Gravity.TOP);
                    infoDialogParams.setMargins(
                            0,
                            targetShape.getPoint().y + targetShape.getHeight() / 2,
                            0,
                            0);
                } else {
                    ((RelativeLayout) infoView).setGravity(Gravity.BOTTOM);
                    infoDialogParams.setMargins(
                            0,
                            0,
                            0,
                            height - (targetShape.getPoint().y + targetShape.getHeight() / 2) + 2 * targetShape.getHeight() / 2);
                }

                infoView.setLayoutParams(infoDialogParams);
                infoView.postInvalidate();

                addView(infoView);

                infoView.setVisibility(VISIBLE);

            }
        });
    }

    private void setDotViewLayout() {
        handler.post(new Runnable() {
            @Override
            public void run() {
                if (dotView.getParent() != null)
                    ((ViewGroup) dotView.getParent()).removeView(dotView);
                RelativeLayout.LayoutParams dotViewLayoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT);
                dotViewLayoutParams.height = dpToPx(Constants.DEFAULT_DOT_SIZE);
                dotViewLayoutParams.width = dpToPx(Constants.DEFAULT_DOT_SIZE);
                dotViewLayoutParams.setMargins(
                        targetShape.getPoint().x - (dotViewLayoutParams.width / 2),
                        targetShape.getPoint().y - (dotViewLayoutParams.height / 2),
                        0,
                        0);
                dotView.setLayoutParams(dotViewLayoutParams);
                dotView.postInvalidate();
                addView(dotView);
                dotView.setVisibility(VISIBLE);
                AnimationFactory.performAnimation(dotView);
            }
        });
    }

    /**
     * SETTERS
     */

    private void setMaskColor(int maskColor) {
        this.maskColor = maskColor;
    }

    private void setDelay(int delayMillis) {
        this.delayMillis = delayMillis;
    }

    private void enableFadeAnimation(boolean isFadeAnimationEnabled) {
        this.isFadeAnimationEnabled = isFadeAnimationEnabled;
    }

    private void setShapeType(ShapeType shape) {
        this.shapeType = shape;
    }

    private void setReady(boolean isReady) {
        this.isReady = isReady;
    }

    private void setTarget(Target target) {
        targetView = target;
    }

    private void setFocusType(Focus focusType) {
        this.focusType = focusType;
    }

    private void setShape(Shape shape) {
        this.targetShape = shape;
    }

    private void setPadding(int padding) {
        this.padding = padding;
    }

    private void setDismissOnTouch(boolean dismissOnTouch) {
        this.dismissOnTouch = dismissOnTouch;
    }

    private void setFocusGravity(FocusGravity focusGravity) {
        this.focusGravity = focusGravity;
    }

    private void setColorTextViewInfo(int colorTextViewInfo) {
        this.colorTextViewInfo = colorTextViewInfo;
        textViewInfo.setTextColor(this.colorTextViewInfo);
    }

    private void setTextViewInfo(String textViewInfo) {
        this.textViewInfo.setText(textViewInfo);
    }

    private void setTextViewTitle(String textViewTitle) {
        this.textViewTitle.setText(textViewTitle);
    }

    private void setTotalShowCaseCount(String totalShowCaseCount) {
        this.textViewTotalShowcaseCount.setText(totalShowCaseCount);
    }

    private void setTextViewInfoSize(int textViewInfoSize) {
        this.textViewInfo.setTextSize(TypedValue.COMPLEX_UNIT_SP, textViewInfoSize);
    }

    private void enableInfoDialog(boolean isInfoEnabled) {
        this.isInfoEnabled = isInfoEnabled;
    }

    private void enableDotView(boolean isDotViewEnabled) {
        this.isDotViewEnabled = isDotViewEnabled;
    }

    public void setConfiguration(MaterialIntroConfiguration configuration) {

        if (configuration != null) {
            this.maskColor = configuration.getMaskColor();
            this.delayMillis = configuration.getDelayMillis();
            this.isFadeAnimationEnabled = configuration.isFadeAnimationEnabled();
            this.colorTextViewInfo = configuration.getColorTextViewInfo();
            this.isDotViewEnabled = configuration.isDotViewEnabled();
            this.dismissOnTouch = configuration.isDismissOnTouch();
            this.colorTextViewInfo = configuration.getColorTextViewInfo();
            this.focusType = configuration.getFocusType();
            this.focusGravity = configuration.getFocusGravity();
        }
    }


    private void setListener(MaterialIntroListener materialIntroListener) {
        this.materialIntroListener = materialIntroListener;
    }

    private void setPerformClick(boolean isPerformClick) {
        this.isPerformClick = isPerformClick;
    }

    private void setButtonClickListener(ButtonClickListener buttonClickListener) {
        this.buttonClickListener = buttonClickListener;
    }

    /**
     * Builder Class
     */
    public static class Builder {

        private MaterialShowCaseView materialShowCaseView;

        private Activity activity;

        private Focus focusType = Focus.MINIMUM;

        public Builder(Activity activity) {
            this.activity = activity;
            materialShowCaseView = new MaterialShowCaseView(activity);
        }

        public Builder setMaskColor(int maskColor) {
            materialShowCaseView.setMaskColor(maskColor);
            return this;
        }

        public Builder setDelayMillis(int delayMillis) {
            materialShowCaseView.setDelay(delayMillis);
            return this;
        }

        public Builder enableFadeAnimation(boolean isFadeAnimationEnabled) {
            materialShowCaseView.enableFadeAnimation(isFadeAnimationEnabled);
            return this;
        }

        public Builder setShape(ShapeType shape) {
            materialShowCaseView.setShapeType(shape);
            return this;
        }

        public Builder setFocusType(Focus focusType) {
            materialShowCaseView.setFocusType(focusType);
            return this;
        }

        public Builder setFocusGravity(FocusGravity focusGravity) {
            materialShowCaseView.setFocusGravity(focusGravity);
            return this;
        }

        public Builder setTarget(View view) {
            materialShowCaseView.setTarget(new ViewTarget(view));
            return this;
        }

        public Builder setTargetPadding(int padding) {
            materialShowCaseView.setPadding(padding);
            return this;
        }

        public Builder setTextColor(int textColor) {
            materialShowCaseView.setColorTextViewInfo(textColor);
            return this;
        }

        public Builder setTotalShowcaseCount(String totalShowCaseCount) {
            materialShowCaseView.setTotalShowCaseCount(totalShowCaseCount);
            return this;
        }

        public Builder setTitleText(String titleText) {
            materialShowCaseView.setTextViewTitle(titleText);
            return this;
        }

        public Builder setInfoText(String infoText) {
            materialShowCaseView.enableInfoDialog(true);
            materialShowCaseView.setTextViewInfo(infoText);
            return this;
        }

        public Builder setInfoTextSize(int textSize) {
            materialShowCaseView.setTextViewInfoSize(textSize);
            return this;
        }

        public Builder dismissOnTouch(boolean dismissOnTouch) {
            materialShowCaseView.setDismissOnTouch(dismissOnTouch);
            return this;
        }

        public Builder enableDotAnimation(boolean isDotAnimationEnabled) {
            materialShowCaseView.enableDotView(isDotAnimationEnabled);
            return this;
        }

        public Builder setConfiguration(MaterialIntroConfiguration configuration) {
            materialShowCaseView.setConfiguration(configuration);
            return this;
        }

        public Builder setListener(MaterialIntroListener materialIntroListener) {
            materialShowCaseView.setListener(materialIntroListener);
            return this;
        }

        public Builder setCustomShape(Shape shape) {
            materialShowCaseView.usesCustomShape = true;
            materialShowCaseView.setShape(shape);
            return this;
        }

        public Builder performClick(boolean isPerformClick) {
            materialShowCaseView.setPerformClick(isPerformClick);
            return this;
        }

        public MaterialShowCaseView build() {
            if (materialShowCaseView.usesCustomShape) {
                return materialShowCaseView;
            }

            // no custom shape supplied, build our own
            Shape shape;

            if (materialShowCaseView.shapeType == ShapeType.CIRCLE) {
                shape = new Circle(
                        materialShowCaseView.targetView,
                        materialShowCaseView.focusType,
                        materialShowCaseView.focusGravity,
                        materialShowCaseView.padding);
            } else {
                shape = new Rect(
                        materialShowCaseView.targetView,
                        materialShowCaseView.focusType,
                        materialShowCaseView.focusGravity,
                        materialShowCaseView.padding);
            }

            materialShowCaseView.setShape(shape);
            return materialShowCaseView;
        }

        public MaterialShowCaseView show() {
            build().show(activity);
            return materialShowCaseView;
        }

        public Builder setOnClickListener(ButtonClickListener buttonClickListener) {
            materialShowCaseView.setButtonClickListener(buttonClickListener);
            return this;
        }
    }
    public interface ButtonClickListener {
        void onClick(boolean next, boolean previous);
    }

}
