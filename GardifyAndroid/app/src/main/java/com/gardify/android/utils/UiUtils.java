package com.gardify.android.utils;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.PopupWindow;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.cardview.widget.CardView;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.navigation.NavController;
import androidx.navigation.Navigation;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.VolleyError;
import com.bumptech.glide.Glide;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.gardify.android.R;
import com.gardify.android.generic.RecycleViewGenericAdapter;
import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.navigation.NavigationView;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.UnsupportedEncodingException;

public class UiUtils {

    private static final String TAG = "UiUtils";

    /**
     * @param activity           activity required to find drawerLayout
     * @param title              is toolbar name, normally change in each child fragment
     * @param backButtonDrawable location of back button icon in drawable
     * @param childActivity      check if childActivity then display back activity, otherwise hides back button
     */
    public static void setupToolbar(Activity activity, String title, int backButtonDrawable, int color, boolean childActivity) {
        DrawerLayout drawer = activity.findViewById(R.id.drawer_layout);
        CardView backBtnCardViewToolbar = drawer.findViewById(R.id.card_view_toolbar_floating_back_button);
        TextView titleToolbar = drawer.findViewById(R.id.text_view_toolbar_title);
        ImageView toolbarGardifyLogo = drawer.findViewById(R.id.image_view_toolbar_gardify_logo);
        ImageView backBtnIconToolbar = drawer.findViewById(R.id.image_view_back_button);

        //set background color
        backBtnCardViewToolbar.setCardBackgroundColor(activity.getResources().getColor(color,null));
        NavController navController = Navigation.findNavController(activity, R.id.nav_host_fragment);

        if (childActivity) {
            toolbarGardifyLogo.setVisibility(View.INVISIBLE);
            backBtnCardViewToolbar.setVisibility(View.VISIBLE);
            //animate
            Animation RightSwipe = AnimationUtils.loadAnimation(activity, R.anim.animation_translate_from_left);
            backBtnCardViewToolbar.startAnimation(RightSwipe);
            titleToolbar.setVisibility(View.VISIBLE);
            titleToolbar.setText(title);
            backBtnIconToolbar.setImageDrawable(activity.getResources().getDrawable(backButtonDrawable, null));

            // pop back-stack if (getBackStackEntryCount() == 0) clear stack and transition to HomeFragment
            backBtnCardViewToolbar.setOnClickListener(v -> {
                activity.onBackPressed();
            });

        } else {
            toolbarGardifyLogo.setVisibility(View.VISIBLE);
            backBtnCardViewToolbar.setVisibility(View.GONE);
            titleToolbar.setVisibility(View.GONE);
        }
    }

    /**
     * @param context allows access to resources
     * @param imageUrl url of the image
     * @param imageView where the image loads into after being retrieved by Glide
     */
    public static void loadImageUsingGlide(Context context, String imageUrl, ImageView imageView) {

        Glide.with(context)
                .load(imageUrl)
                .placeholder(R.drawable.gardify_image_placeholder)
                .thumbnail(0.05f)
                .centerCrop()
                .diskCacheStrategy(DiskCacheStrategy.AUTOMATIC)
                .dontAnimate()
                .dontTransform()
                .into(imageView);
    }

    /**
     * @param context required to find main bottomNavigation
     * @param newMenuId new bottomNavigation menu with icons
     * @param onNavigationItemSelectedListener bottomNavigation button Click listeners
     */
    public static void changeBottomNavigationView(Context context, int newMenuId, BottomNavigationView.OnNavigationItemSelectedListener onNavigationItemSelectedListener) {
        Activity activity = (Activity) context;
        BottomNavigationView communityBottomNavigation = activity.findViewById(R.id.main_bottomnav);
        communityBottomNavigation.getMenu().clear(); //clear old inflated items.
        communityBottomNavigation.inflateMenu(newMenuId);
        communityBottomNavigation.setOnNavigationItemSelectedListener(onNavigationItemSelectedListener);
    }

    /**
     * shouldOverrideUrlLoading method loads a html String to Web View
     */
    public static class AppWebViewClients extends WebViewClient {
        private ProgressBar progressBar;

        public AppWebViewClients(ProgressBar progressBar) {
            this.progressBar = progressBar;
            progressBar.setVisibility(View.VISIBLE);
        }

        @Override
        public boolean shouldOverrideUrlLoading(WebView view, String htmlString) {
            view.loadDataWithBaseURL(null, htmlString, "text/html", "UTF-8", null);
            return true;
        }

        @Override
        public void onPageFinished(WebView view, String url) {
            super.onPageFinished(view, url);
            progressBar.setVisibility(View.GONE);
        }
    }


    /**
     * @param context updates Drawer menu footer based on user login status
     */
    public static void updateNavFooterBasedOnLoginStatus(Context context) {
        Activity activity = (Activity) context;

        NavigationView drawerNavFooter = activity.findViewById(R.id.drawer_nav_view_footer);
        //clear previous menu
        drawerNavFooter.getMenu().clear();
        if (PreferencesUtility.getLoggedIn(context)) {
            drawerNavFooter.inflateMenu(R.menu.menu_activitymain_drawerfooterlogin);
        } else {
            drawerNavFooter.inflateMenu(R.menu.menu_activitymain_drawerfooter);
        }
    }

    /**
     * @param context  will be used to inflate layout
     * @param view     indicates where popup window is shown
     * @param infoText text to be set inside popup_menu_default layout TextView
     */
    public static void displayInfoDropDownMenu(Context context, View view, String infoText) {


        LayoutInflater layoutInflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        final View popupView = layoutInflater.inflate(R.layout.popup_menu_default, null);

        PopupWindow popupWindow = new PopupWindow(
                popupView,
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);

        popupWindow.setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
        popupWindow.setElevation(20);
        popupWindow.setOutsideTouchable(true);

        TextView textInfo = popupView.findViewById(R.id.text_view_default_popup_menu);
        textInfo.setText(infoText);

        popupWindow.showAsDropDown(view, -20, 0);
    }

    /**
     * @param context  will be used to inflate layout
     * @param infoText string to be set inside dialog window TextView
     */

    public static void displayAlertDialog(Context context, String infoText) {
        Activity activity = (Activity) context;
        AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(activity);
        LayoutInflater inflater = activity.getLayoutInflater();

        //initialize views
        View dialogView = inflater.inflate(R.layout.popup_dialog_default, null);
        TextView textInfo = dialogView.findViewById(R.id.text_view_default_popup_dialog);
        Button closeBtn = dialogView.findViewById(R.id.button_popup_dialog_close);

        // set text
        textInfo.setText(infoText);

        // show dialog
        dialogBuilder.setView(dialogView);
        AlertDialog alertDialog = dialogBuilder.create();
        alertDialog.setCancelable(true);
        alertDialog.show();

        // close button click listener
        closeBtn.setOnClickListener(v -> {
            alertDialog.dismiss();
        });
    }

    /**
     * @param error get response body and parse with appropriate encoding
     */
    public static void showErrorDialogNetworkParsed(Context context, VolleyError error) {
        if (error == null || error.networkResponse == null) {
            return;
        }
        try {
            String body = new String(error.networkResponse.data,"UTF-8");

            // parse Message
            JSONObject obj = new JSONObject(body);
            String errorMessage = obj.getString("Message");
            displayAlertDialog(context, errorMessage);

        } catch (UnsupportedEncodingException e) {
            // exception
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    /**
     * @param context      will be used to access resources.
     * @param recyclerView RecycleView to populate
     * @param adapter      generic RecycleView adapter
     * @param Orientation  RecycleView orientation can be set to RecyclerView.VERTICAL or RecyclerView.Horizontal
     */
    public static void configGenericRecyclerView(Context context, RecyclerView recyclerView,
                                                 RecycleViewGenericAdapter adapter, int Orientation) {
        LinearLayoutManager layoutManager = new LinearLayoutManager(context, Orientation, false);
        recyclerView.setLayoutManager(layoutManager);
        recyclerView.setAdapter(adapter);
    }

    /**
     * @param destinationFragment the Fragment to replace current one
     * @param activity    normally pass getActivity(), required for getSupportFragmentManager()
     * @param clearStack  clears previously stacked fragments if set to true
     * @param bundle  data for next fragment can be placed in a Bundle, in case of no data parameter should be null
     */
    public static void navigateToFragment(int destinationFragment, Activity activity, boolean clearStack, Bundle bundle) {
        NavController navController = Navigation.findNavController(activity, R.id.nav_host_fragment);
        DrawerLayout drawer = activity.findViewById(R.id.drawer_layout);
        ImageView toolbarGardifyLogo = drawer.findViewById(R.id.image_view_toolbar_gardify_logo);
        //clear fragment backStack then begin transaction
        if (clearStack) {
            navController.popBackStack(R.id.nav_controller_home,false);
            toolbarGardifyLogo.setImageResource(R.drawable.gardify_logo_header);
        }
        Navigation.findNavController(activity, R.id.nav_host_fragment).navigate(destinationFragment, bundle);
    }

    public static void setToolbarName(Activity activity, String title, boolean childActivity){
        DrawerLayout drawer = activity.findViewById(R.id.drawer_layout);
        TextView titleToolbar = drawer.findViewById(R.id.text_view_toolbar_title);
        ImageView toolbarGardifyLogo = drawer.findViewById(R.id.image_view_toolbar_gardify_logo);

        if (childActivity) {
            toolbarGardifyLogo.setImageResource(R.drawable.gardify_app_logo_weiss_ohne_schrift);
            titleToolbar.setVisibility(View.VISIBLE);
            titleToolbar.setText(title);
        } else {
            activity.onBackPressed();
            toolbarGardifyLogo.setImageResource(R.drawable.gardify_logo_header);
            navigateToFragment(R.id.nav_controller_home, activity, true,null);
            titleToolbar.setVisibility(View.GONE);
        }
    }


    /**
     * @param view disable animation and shadow by turning
     *             off hardware acceleration for particular view
     */
    public static void disableHardWareAccelerationForView(View view) {
        view.setLayerType(View.LAYER_TYPE_SOFTWARE, null);
    }
}
