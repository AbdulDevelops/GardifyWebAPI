package com.gardify.android.ui;

import android.content.Intent;
import android.content.IntentSender;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.content.res.ResourcesCompat;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.navigation.NavController;
import androidx.navigation.Navigation;
import androidx.navigation.ui.AppBarConfiguration;
import androidx.navigation.ui.NavigationUI;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.BuildConfig;
import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.myGarden.PlantCount;
import com.gardify.android.data.todos.TodoCount;
import com.gardify.android.data.warning.Warning;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.navigation.NavigationView;
import com.google.android.material.snackbar.Snackbar;
import com.google.android.play.core.appupdate.AppUpdateInfo;
import com.google.android.play.core.appupdate.AppUpdateManager;
import com.google.android.play.core.appupdate.AppUpdateManagerFactory;
import com.google.android.play.core.install.InstallStateUpdatedListener;
import com.google.android.play.core.install.model.AppUpdateType;
import com.google.android.play.core.install.model.InstallStatus;
import com.google.android.play.core.install.model.UpdateAvailability;
import com.google.android.play.core.tasks.Task;

import org.jetbrains.annotations.NotNull;

import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.updateNavFooterBasedOnLoginStatus;


public class MainActivity extends AppCompatActivity implements View.OnClickListener {

    private static final String ACTION_ARGUMENT = "action_argument";
    private static final int UPDATE_REQUEST_CODE = 1001;
    private static final String TAG = "MainActivity";

    // drawer
    private DrawerLayout drawer;
    private NavigationView drawerNavView, drawerNavViewFooter;
    private NavController navController;
    private BottomNavigationView bottomNavigationView;
    private AppBarConfiguration mAppBarConfiguration;
    private TextView todoCountTxt, todoCountLabel,
            plantCountTxt, plantCountLabel,
            warningCountTxt, warningCountLabel;
    AppUpdateManager appUpdateManager;
    boolean splashRegisterClick = false;
    private boolean connectionPossible;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        checkForBundleContent();

        // initialize views
        Initialize();

        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawer, toolbar, R.string.mainActivity_navigationDrawerOpen, R.string.mainActivity_navigationDrawerClose);
        drawer.addDrawerListener(toggle);
        toggle.syncState();

        // handle navigation drawer menu click events
        drawerNavView.setNavigationItemSelectedListener(getOnNavigationItemSelectedListener(drawer, navController));

        // handle navigation drawer footer menu click events
        drawerNavViewFooter.setNavigationItemSelectedListener(getOnNavigationFooterItemSelectedListener(drawer, navController));

        // close drawer layout after navigation item is selected
        navController.addOnDestinationChangedListener(closeDrawer());

        // bottom navigation click events listener
        bottomNavigationView.setOnNavigationItemSelectedListener(BottomNavigationClickListener());

        //update actionbar counters
        initializeActionbar();

        checkForUpdates();

        //show beta version message to first time users
        if (PreferencesUtility.getIsFirstTimeUser(this)) {
            displayAlertDialog(this, getResources().getString(R.string.mainActivity_firstTimeUserMessage));

            PreferencesUtility.setIsFirstTimeUser(this, false);
        }

        checkConnectionToBackend();
    }

    private void Initialize() {
        drawer = findViewById(R.id.drawer_layout);
        drawerNavView = findViewById(R.id.drawer_nav_view);
        drawerNavViewFooter = findViewById(R.id.drawer_nav_view_footer);
        navController = Navigation.findNavController(this, R.id.nav_host_fragment);
        bottomNavigationView = (BottomNavigationView) findViewById(R.id.main_bottomnav);

        updateNavFooterBasedOnLoginStatus(this);
    }

    private void checkForBundleContent() {
        Intent intent = getIntent();
        try {
            splashRegisterClick = intent.getExtras().getBoolean("splashRegisterClick");
        } catch (Exception e) {
            Log.d(TAG, "onCreate: " + e.toString());
        }

        Bundle registerBundle = new Bundle();

        if(intent.hasExtra("splashRegisterClick")) {
            if (splashRegisterClick) {
                registerBundle.putBoolean("help", splashRegisterClick);
                Log.d(TAG, "onCreate: " + registerBundle);
                navigateToFragment(R.id.nav_controller_login, this, true, registerBundle);
            } else {
                registerBundle.putBoolean("help", splashRegisterClick);
                navigateToFragment(R.id.nav_controller_login, this, true, registerBundle);
            }
        }
    }

    @NotNull
    private NavigationView.OnNavigationItemSelectedListener getOnNavigationItemSelectedListener(DrawerLayout drawer, NavController navController) {
        return menuItem -> {
            switch (menuItem.getItemId()) {
                case R.id.nav_my_garden:

                    navigateToFragment(R.id.nav_controller_my_garden, this, true, null);
                    break;

                case R.id.nav_todo:

                    navigateToFragment(R.id.nav_controller_todo, this, true, null);

                    break;
                case R.id.nav_plant_search:

                    navigateToFragment(R.id.nav_controller_plant_search, this, true, null);

                    break;
                case R.id.nav_plant_scan:

                    navigateToFragment(R.id.nav_controller_plant_scan, this, true, null);

                    break;
                case R.id.nav_news:

                    navigateToFragment(R.id.nav_controller_news, this, true, null);

                    break;
                case R.id.nav_gardify_video:

                    navigateToFragment(R.id.nav_controller_gardify_video, this, true, null);

                    break;
                case R.id.nav_plant_doc:

                    navigateToFragment(R.id.nav_controller_plant_doc, this, true, null);

                    break;
                case R.id.nav_weather:

                    navigateToFragment(R.id.nav_controller_weather, this, true, null);

                    break;
                case R.id.nav_shop:

                    displayAlertDialog(this, "Demnächst verfügbar");

                    break;
                case R.id.nav_eco_scan:

                    navigateToFragment(R.id.nav_controller_eco_scan, this, true, null);

                    break;
                case R.id.nav_guided_tour:

                    //Passing arguments
                    Bundle args = new Bundle();
                    args.putInt(ACTION_ARGUMENT, R.string.all_guidedTour);
                    // change fragment
                    navigateToFragment(R.id.nav_controller_home, this, true, args);

                    break;
                case R.id.nav_garden_glossary:

                    navigateToFragment(R.id.nav_controller_garden_glossary, this, true, null);

                    break;
                case R.id.nav_add_plants:

                    navigateToFragment(R.id.nav_controller_suggest_plants, this, true, null);

                    break;

                case R.id.nav_gardify_plus:

                    displayAlertDialog(this, "Demnächst verfügbar");

                    break;

            }
            //This is for closing the drawer after acting on it
            drawer.closeDrawers();
            return false;
        };
    }

    @NotNull
    private BottomNavigationView.OnNavigationItemSelectedListener BottomNavigationClickListener() {
        return item -> {
            switch (item.getItemId()) {
                case R.id.bottom_navigation_home:
                    navigateToFragment(R.id.nav_controller_home, this, true, null);

                    break;
                case R.id.bottom_navigation_settings:
                    navigateToFragment(R.id.nav_controller_settings, this, true, null);

                    break;
            }
            return true;
        };
    }

    @NotNull
    private NavigationView.OnNavigationItemSelectedListener getOnNavigationFooterItemSelectedListener(DrawerLayout drawer, NavController navController) {
        return menuItem -> {
            switch (menuItem.getItemId()) {
                case R.id.nav_login:

                    // delete login details
                    PreferencesUtility.setLoggedOut(this);
                    navigateToFragment(R.id.nav_controller_login, this, true, null);

                    updateNavFooterBasedOnLoginStatus(this);
                    break;

                case R.id.nav_logout:

                    // delete login details
                    PreferencesUtility.setLoggedOut(this);

                    updateNavFooterBasedOnLoginStatus(this);
                    //refresh activity for changes to take effect
                    navigateToFragment(R.id.nav_controller_login, this, true, null);

                    break;

                case R.id.nav_newsletter:
                    navigateToFragment(R.id.nav_controller_newsletter, this, true, null);

                    break;

                case R.id.nav_imprint:
                    navigateToFragment(R.id.nav_controller_imprint, this, true, null);

                    break;

                case R.id.nav_privacy_policy:
                    navigateToFragment(R.id.nav_controller_privacy_policy, this, true, null);

                    break;

                case R.id.nav_team:
                    navigateToFragment(R.id.nav_controller_team, this, true, null);

                    break;

                case R.id.nav_agb:
                    navigateToFragment(R.id.nav_controller_agb, this, true, null);

                    break;

                case R.id.nav_settings:
                    navigateToFragment(R.id.nav_controller_settings, this, true, null);

                    break;

                case R.id.nav_contact:
                    navigateToFragment(R.id.nav_controller_contact, this, true, null);

                    break;

                case R.id.nav_bestellungen:
                case R.id.nav_shop:
                case R.id.nav_wunschliste:
                case R.id.nav_warenkorb:

                    displayAlertDialog(this, "Demnächst verfügbar");
                    break;

            }

            //This is for closing the drawer after acting on it
            drawer.closeDrawers();
            return false;
        };
    }

    private void initializeActionbar() {
        todoCountTxt = findViewById(R.id.todo_count_appbar_main);
        todoCountLabel = findViewById(R.id.todo_count_appbar_main_label);
        plantCountTxt = findViewById(R.id.plant_count_appbar_main);
        plantCountLabel = findViewById(R.id.plant_count_appbar_main_label);
        warningCountTxt = findViewById(R.id.warning_count_appbar_main);
        warningCountLabel = findViewById(R.id.warning_count_appbar_main_label);

        todoCountTxt.setOnClickListener(this);
        todoCountLabel.setOnClickListener(this);
        plantCountTxt.setOnClickListener(this);
        plantCountLabel.setOnClickListener(this);
        warningCountTxt.setOnClickListener(this);
        warningCountLabel.setOnClickListener(this);

        updateActionBarCounters();


    }

    public void updateActionBarCounters() {
        DisplayActionbarCounters();

        String todoCountUrl = APP_URL.TODO_COUNT_API;
        RequestQueueSingleton.getInstance(this).typedRequest(todoCountUrl, this::onSuccessTodoCount, null, TodoCount.class, new RequestData(RequestType.TodoCount));
    }

    private void DisplayActionbarCounters() {

        PlantCount plantCount = PreferencesUtility.getPlantCount(this);
        TodoCount todoCount = PreferencesUtility.getTodoCount(this);
        String countPref = PreferencesUtility.getWarningCount(this);
        int warningCount = Integer.parseInt(TextUtils.isEmpty(countPref) ? "0" : countPref);

        if (plantCount != null)
            plantCountTxt.setText(plantCount.getSorts() + "/" + plantCount.getTotal());
        if (todoCount != null)
            todoCountTxt.setText(todoCount.getAllTodosOfTheMonth() + "/" + todoCount.getAllTodos());

        if (warningCount > 0) {
            // set warning bg color to red
            warningCountTxt.setBackground(ResourcesCompat.getDrawable(getResources(), R.drawable.red_circle, null));
            warningCountTxt.setText(String.valueOf(warningCount));
        } else {
            warningCountTxt.setText("0");
            warningCountTxt.setBackground(null);
        }
    }

    private void onSuccessTodoCount(TodoCount model, RequestData data) {
        todoCountTxt.setText(model.getAllTodosOfTheMonth() + "/" + model.getAllTodos());

        //set preferences
        PreferencesUtility.setTodoCount(this, model);

        String PlantCountUrl = APP_URL.USER_GARDEN_API + "count";
        RequestQueueSingleton.getInstance(this).typedRequest(PlantCountUrl, this::onSuccessPlantCount, null, PlantCount.class, new RequestData(RequestType.PlantCount));

    }

    private void onSuccessPlantCount(PlantCount model, RequestData data) {
        plantCountTxt.setText(model.getSorts() + "/" + model.getTotal());
        //set preferences
        PreferencesUtility.setPlantCount(this, model);

        String warningApiUrl = APP_URL.WARNING_API + "warnings";
        RequestQueueSingleton.getInstance(this).typedRequest(warningApiUrl, this::onSuccessWarningCount, null, Warning[].class, new RequestData(RequestType.Warning));

    }

    private void onSuccessWarningCount(Warning[] warnings, RequestData data) {
        int warningCount = 0;
        for (Warning warning : warnings) {
            if (!warning.getDismissed()) {
                warningCount++;
            }
        }
        //set preferences
        PreferencesUtility.setWarningCount(this, String.valueOf(warningCount));

        //update counters
        DisplayActionbarCounters();
    }

    @Override
    public boolean onSupportNavigateUp() {
        NavController navController = Navigation.findNavController(this, R.id.nav_host_fragment);
        return NavigationUI.navigateUp(navController, mAppBarConfiguration)
                || super.onSupportNavigateUp();
    }

    @Override
    public void onClick(View view) {

        switch (view.getId()) {
            case R.id.todo_count_appbar_main:
            case R.id.todo_count_appbar_main_label:
                navigateToFragment(R.id.nav_controller_todo, this, true, null);

                break;
            case R.id.plant_count_appbar_main:
            case R.id.plant_count_appbar_main_label:
                navigateToFragment(R.id.nav_controller_my_garden, this, true, null);

                break;
            case R.id.warning_count_appbar_main:
            case R.id.warning_count_appbar_main_label:
                navigateToFragment(R.id.nav_controller_warning, this, true, null);

                break;
        }

    }

    @NotNull
    private NavController.OnDestinationChangedListener closeDrawer() {
        return (controller, destination, arguments) -> {
            if (drawer.isDrawerOpen(GravityCompat.START))
                drawer.closeDrawer(GravityCompat.START, true);
        };
    }

    private void checkConnectionToBackend() {
        //InetAddress gardifyAddress = InetAddress.getByName(APP_URL.PLANT_SEARCH_TOTAL_COUNT);
        RequestQueueSingleton.getInstance(MainActivity.this).arrayRequest(APP_URL.PLANT_FAMILY, Request.Method.GET, null , this::onErrorConnection, null);
    }

    private void onErrorConnection(VolleyError volleyError) {
        Log.d(TAG, "onErrorConnection: " + volleyError.networkResponse.toString());
        createNonCancelableConnectionAlert();
    }

    private void createNonCancelableConnectionAlert() {
        AlertDialog.Builder connectionAlert = new AlertDialog.Builder(MainActivity.this);
        LayoutInflater inflater = getLayoutInflater();

        View dialogView = inflater.inflate(R.layout.popup_dialog_default, null);
        TextView textInfo = dialogView.findViewById(R.id.text_view_default_popup_dialog);
        Button closeBtn = dialogView.findViewById(R.id.button_popup_dialog_close);
        closeBtn.setText(getResources().getString(R.string.mainActivity_understood));

        textInfo.setText(getResources().getString(R.string.mainActivity_noConnectionMaintenance));

        connectionAlert.setView(dialogView);

        AlertDialog alertDialog = connectionAlert.create();
        alertDialog.setCancelable(false);
        alertDialog.show();

        closeBtn.setOnClickListener(v -> {
            alertDialog.dismiss();
            finish();
        });
    }

    public void checkForUpdates() {
        if (!BuildConfig.DEBUG) { // check for update only in release mode

            appUpdateManager = AppUpdateManagerFactory.create(MainActivity.this);
            Task<AppUpdateInfo> appUpdateInfoTask = appUpdateManager.getAppUpdateInfo();
            appUpdateInfoTask.addOnSuccessListener(appUpdateInfo -> {
                if (appUpdateInfo.updateAvailability() == UpdateAvailability.UPDATE_AVAILABLE
                        && appUpdateInfo.isUpdateTypeAllowed(AppUpdateType.FLEXIBLE)) {
                    // If an in-app update is already running, resume the update.
                    try {
                        appUpdateManager.startUpdateFlowForResult(
                                appUpdateInfo,
                                AppUpdateType.IMMEDIATE,
                                this,
                                UPDATE_REQUEST_CODE);
                    } catch (IntentSender.SendIntentException e) {
                        e.printStackTrace();
                        //Toast.makeText(this, "FEHLER: " + e.toString(), Toast.LENGTH_SHORT).show();
                        Log.d(TAG, "checkForUpdates: Error");

                    }
                } else
                    Log.d(TAG, "checkForUpdates: NO UPDATES");
                //Toast.makeText(this, "Keine Updates", Toast.LENGTH_LONG).show();

                Log.d(TAG, "checkForUpdates: NO UPDATES");
            }).addOnFailureListener(e -> {
                Log.w(TAG, "onFailure: Fehler", e);
                //Toast.makeText(this, "Fehler: " + e.toString(), Toast.LENGTH_LONG).show();

            });
            appUpdateManager.registerListener(installStateUpdatedListener);

        }
    }

    @Override
    public void onStop() {
        if (appUpdateManager != null)
            appUpdateManager.unregisterListener(installStateUpdatedListener);
        super.onStop();
    }

    private final InstallStateUpdatedListener installStateUpdatedListener = state -> {
        if (state.installStatus() == InstallStatus.DOWNLOADED) {
            showCompletedUpdate();
        }
    };

    private void showCompletedUpdate() {
        Snackbar snackbar = Snackbar.make(findViewById(R.id.main_bottomnav), R.string.mainActivity_newVersionReady, Snackbar.LENGTH_INDEFINITE);
        snackbar.setAction(R.string.mainActivity_install, v -> appUpdateManager.completeUpdate()).show();
    }
}