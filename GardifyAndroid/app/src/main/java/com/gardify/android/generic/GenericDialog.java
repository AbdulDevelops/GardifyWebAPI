package com.gardify.android.generic;

import android.annotation.SuppressLint;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Typeface;
import androidx.annotation.DrawableRes;
import androidx.annotation.FontRes;
import androidx.annotation.StyleRes;
import androidx.core.content.res.ResourcesCompat;
import androidx.appcompat.app.AlertDialog;

import android.util.TypedValue;
import android.view.ContextThemeWrapper;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.gardify.android.R;
import com.google.android.material.button.MaterialButton;

import java.util.ArrayList;
import java.util.List;


public class GenericDialog {

    private String title, message;
    private int icon, titleTextColor, messageTextColor, buttonOrientation, dialogFont, dialogTheme;
    private int titleTextSize, messageTextSize;
    private boolean isDialogCancelable;
    private Bitmap image;

    private GenericDialog(Builder builder) {
        this.title = builder.title;
        this.message = builder.message;
        this.icon = builder.icon;
        this.image = builder.bitmapImage;
        this.titleTextColor = builder.titleTextColor;
        this.messageTextColor = builder.messageTextColor;
        this.buttonOrientation = builder.buttonOrientation;
        this.dialogFont = builder.dialogFont;
        this.isDialogCancelable = builder.isDialogCancelable;
        this.titleTextSize = builder.titleTextSize;
        this.messageTextSize = builder.messageTextSize;
        this.dialogTheme = builder.dialogTheme;
    }

    public static class Builder {

        private String title, message;
        private int icon, titleTextColor, messageTextColor, dialogFont, buttonOrientation, dialogTheme;
        private int titleTextSize, messageTextSize;
        private boolean isDialogCancelable;
        private Bitmap bitmapImage;
        private View view;
        private Context context;
        private AlertDialog.Builder dialog;
        private List<Button> buttonList = new ArrayList<>();
        private Typeface typeface;

        public Builder(Context context) {
            this.context = context;
        }

        public Builder setTitle(String title) {
            this.title = title;
            return this;
        }

        public Builder setMessage(String message) {
            this.message = message;
            return this;
        }

        public Builder setIcon(@DrawableRes int icon) {
            this.icon = icon;
            return this;
        }

        public Builder setBitmapImage(Bitmap bitmapImage) {
            this.bitmapImage = bitmapImage;
            return this;
        }

        public Builder setTitleAppearance(int titleTextColor, int titleTextSize) {
            this.titleTextColor = titleTextColor;
            this.titleTextSize = titleTextSize;
            return this;
        }

        public Builder setMessageAppearance(int messageTextColor, int messageTextSize) {
            this.messageTextColor = messageTextColor;
            this.messageTextSize = messageTextSize;
            return this;
        }

        LinearLayout llButtonContainer;
        TextView txtTitle;
        TextView txtMessage;
        ImageView ivIcon, ivImage;
        AlertDialog displayDialog;

        @SuppressLint("InflateParams")
        public GenericDialog generate() {
            if (dialogTheme != 0) {
                dialog = new AlertDialog.Builder(new ContextThemeWrapper(context, dialogTheme));
            } else {
                dialog = new AlertDialog.Builder(context);
            }
            dialog.setCancelable(true);
            view = LayoutInflater.from(context).inflate(R.layout.generic_dialog_layout, null);
            dialog.setView(view);
            initViews();
            dialog.setCancelable(isDialogCancelable);
            displayDialog = dialog.show();
            return new GenericDialog(this);
        }

        private void initViews() {
            llButtonContainer = view.findViewById(R.id.ll_button_container);
            txtTitle = view.findViewById(R.id.txt_title);
            txtMessage = view.findViewById(R.id.txt_message);
            ivIcon = view.findViewById(R.id.iv_icon);
            ivImage = view.findViewById(R.id.iv_image);

            displayFont();

            displayTitle();

            displayMessage();

            displayIcon();

            displayImage();

            displayButtons();
        }

        private void displayButtons() {
            llButtonContainer.setOrientation(buttonOrientation);
            for (int i = 0; i < buttonList.size(); i++) {
                llButtonContainer.addView(buttonList.get(i));
            }
        }

        private void displayIcon() {
            if (icon != 0) {
                ivIcon.setVisibility(View.VISIBLE);
                ivIcon.setImageResource(icon);
            }
        }

        private void displayImage() {
            if (bitmapImage != null) {
                ivImage.setVisibility(View.VISIBLE);
                ivImage.setImageBitmap(bitmapImage);
            }
        }

        private void displayMessage() {
            if (message != null) {
                txtMessage.setVisibility(View.VISIBLE);
                txtMessage.setText(message);
                if (messageTextColor != 0) {
                    txtMessage.setTextColor(ResourcesCompat.getColor(context.getResources(), messageTextColor, null));
                }
                if (messageTextSize != 0) {
                    txtMessage.setTextSize(TypedValue.COMPLEX_UNIT_PX, context.getResources().getDimension(messageTextSize));
                }
                if (typeface != null) {
                    txtMessage.setTypeface(typeface);
                }
            }
        }

        private void displayTitle() {
            if (title != null) {
                txtTitle.setVisibility(View.VISIBLE);
                txtTitle.setText(title);
                if (titleTextColor != 0) {
                    txtTitle.setTextColor(ResourcesCompat.getColor(context.getResources(), titleTextColor, null));
                }
                if (titleTextSize != 0) {
                    txtTitle.setTextSize(TypedValue.COMPLEX_UNIT_PX, context.getResources().getDimension(titleTextSize));
                }
                if (typeface != null) {
                    txtTitle.setTypeface(typeface);
                }
            }
        }

        private void displayFont() {
            if (dialogFont != 0) {
                typeface = ResourcesCompat.getFont(context, dialogFont);
            }
        }

        public Builder addNewButton(@StyleRes int style, String buttonText, int textSizeId, final GenericDialogOnClickListener addBtnListener) {
            MaterialButton addButton = new MaterialButton(new ContextThemeWrapper(context, style), null, style);
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1f);
            layoutParams.setMargins(8, 8, 8, 8);
            addButton.setLayoutParams(layoutParams);
            addButton.setTypeface(typeface);
            addButton.setText(buttonText);
            addButton.setTextSize(TypedValue.COMPLEX_UNIT_PX, context.getResources().getDimension(textSizeId));
            addButton.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    addBtnListener.onClick(view);
                    displayDialog.dismiss();
                }
            });
            buttonList.add(addButton);
            return this;
        }

        public Builder setButtonOrientation(int orientation) {
            this.buttonOrientation = orientation;
            return this;
        }

        public Builder setDialogFont(@FontRes int font) {
            this.dialogFont = font;
            return this;
        }

        public Builder setCancelable(boolean cancel) {
            this.isDialogCancelable = cancel;
            return this;
        }

        public Builder setDialogTheme(@StyleRes int genericDialogTheme) {
            this.dialogTheme = genericDialogTheme;
            return this;
        }
    }

    public  interface GenericDialogOnClickListener {
        void onClick(View view);
    }
}