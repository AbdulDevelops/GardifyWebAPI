package com.gardify.android.ui.ecoScan;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.LinearGradient;
import android.graphics.Shader;
import android.graphics.drawable.Drawable;
import android.graphics.drawable.ShapeDrawable;
import android.graphics.drawable.shapes.RoundRectShape;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.util.Base64;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.ScrollView;
import android.widget.SeekBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.data.account.UserMainGarden;
import com.gardify.android.data.ecoScan.DurationRatingPlant;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.settings.UserInfo;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.gardify.android.utils.TimeUtils;
import com.github.mikephil.charting.charts.CombinedChart;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.data.BarData;
import com.github.mikephil.charting.data.BarDataSet;
import com.github.mikephil.charting.data.BarEntry;
import com.github.mikephil.charting.data.CombinedData;
import com.github.mikephil.charting.data.Entry;
import com.github.mikephil.charting.data.LineData;
import com.github.mikephil.charting.data.LineDataSet;
import com.github.mikephil.charting.formatter.ValueFormatter;
import com.github.mikephil.charting.interfaces.datasets.IBarDataSet;

import org.jetbrains.annotations.NotNull;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static com.gardify.android.utils.APP_URL.isAndroid;
import static com.gardify.android.utils.ImageUtils.getImageBytes;
import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.displayInfoDropDownMenu;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class EcoScanFragment extends Fragment implements View.OnClickListener, TextWatcher {

    private static final String TAG = "EcoScanFragment";
    public static final String SCREENSHOT_ECOSCAN_KEY = "SCREENSHOT";
    public static final int BITMAP_A4_PAGE_HEIGHT = 2048;
    public static final int CATEGORY_1 = 1;
    public static final int CATEGORY_2 = 2;
    public static final int CATEGORY_3 = 3;
    public static final int CATEGORY_4 = 4;
    public static final float CATEGORY_1_RED = 0.02f;
    public static final float CATEGORY_1_YELLOW_START = 0.10f;
    public static final float CATEGORY_1_YELLOW_END = 0.32f;
    public static final float CATEGORY_1_GREEN = 0.40f;
    public static final float CATEGORY_2_RED = 0.20f;
    public static final float CATEGORY_2_YELLOW_START = 0.35f;
    public static final float CATEGORY_2_YELLOW_END = 0.55f;
    public static final float CATEGORY_2_GREEN = 0.60f;
    public static final float CATEGORY_3_RED = 0.35f;
    public static final float CATEGORY_3_YELLOW_START = 0.45f;
    public static final float CATEGORY_3_YELLOW_END = 0.65f;
    public static final float CATEGORY_3_GREEN = 0.70f;
    public static final float CATEGORY_4_RED = 0.45f;
    public static final float CATEGORY_4_YELLOW_START = 0.55f;
    public static final float CATEGORY_4_YELLOW_END = 0.75f;
    public static final float CATEGORY_4_GREEN = 0.80f;
    public static final float DEFAULT_RED = 0.1f;
    public static final float DEFAULT_YELLOW_START = 0.40f;
    public static final float DEFAULT_YELLOW_END = 0.50f;
    public static final float DEFAULT_GREEN = 0.7f;
    public static final float DEFAULT_ECO_ELEMENT_RED = 0.02f;
    public static final float DEFAULT_ECO_ELEMENT_YELLOW_START = 0.10f;
    public static final float DEFAULT_ECO_ELEMENT_YELLOW_END = 0.25f;
    public static final float DEFAULT_ECO_ELEMENT_GREEN = 0.35f;
    public static final float DEFAULT_DIVERSITY_RED = 0.10f;
    public static final float DEFAULT_DIVERSITY_YELLOW_START = 0.20f;
    public static final float DEFAULT_DIVERSITY_YELLOW_END = 0.40f;
    public static final float DEFAULT_DIVERSITY_GREEN = 50f;
    private TextView gardenOwnerTxt, ecoScanDateLabel, ecoScanAboutDescLabel,
            surfaceAreaLabelTxt, greenAreaLabelTxt;

    private ScrollView mainScrollView;
    private LinearLayout mainLinearLayout;
    private EditText surfaceAreaEdtText, greenAreaEdtText;

    private ImageView landUseInfoImg, ecoElementInfo, plantDiversityInfo, floweringTimeInfo, expandDescIcon;

    private SeekBar landUseSeekBar, ecoElementSeekBar, plantDiversitySeekBar;

    private Button saveToCalendarBtn, shareResultBtn;
    private ProgressBar progressBar;

    private CombinedChart combinedChart;

    private String gardenRating, plantsRating;
    private double landUseArea = 0, landArea, greenArea;
    private double areaRating = 0;
    private List<DurationRatingPlant> durationRatingPlantList = new ArrayList<>();
    private Integer[] lineChartData = {2, 4, 6, 7, 15, 20, 20, 15, 10, 6, 4, 2};

    private String[] Months = {"Jan.", "Feb.", "März", "April", "Mai", "Juni", "Juli", "Aug.", "Sept.", "Okt.", "Nov.", "Dez."};
    private Integer[] plantCount;

    boolean isExpandDesc = false;

    // mpChart variables
    private BarData barData;
    private LineData lineData;
    private CombinedData combinedData;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_eco_scan, container, false);
        //setup Toolbar
        setupToolbar(getActivity(), "GARTEN ÖKOSCAN", R.drawable.gardify_app_icon_oekoscan, R.color.toolbar_gardenEcoScan_setupToolbar, true);

        authenticationCheck();

        init(root);
        setupInitialUI();
        restorePreviousState();
        setupSeekBarGradients();

        getDurationRatingFromApi();

        return root;
    }

    public void init(View root) {

        //linearLayout
        mainScrollView = root.findViewById(R.id.scrollView_ecoScan);
        expandDescIcon = root.findViewById(R.id.expand_icon_eco_scan_desc);
        mainLinearLayout = root.findViewById(R.id.linear_layout_eco_scan_top_view);
        //TextView
        gardenOwnerTxt = root.findViewById(R.id.text_view_eco_scan_garden_owner_label);
        ecoScanDateLabel = root.findViewById(R.id.text_view_eco_scan_date_label);
        ecoScanAboutDescLabel = root.findViewById(R.id.text_view_eco_scan_about_desc);
        surfaceAreaLabelTxt = root.findViewById(R.id.text_view_eco_scan_surface_label);
        greenAreaLabelTxt = root.findViewById(R.id.text_view_eco_scan_green_area_label);

        //EditText
        surfaceAreaEdtText = root.findViewById(R.id.editText_eco_scan_surface);
        greenAreaEdtText = root.findViewById(R.id.editText_eco_scan_green_area);

        //ImageView
        landUseInfoImg = root.findViewById(R.id.image_view_eco_scan_land_use_info);
        ecoElementInfo = root.findViewById(R.id.image_view_eco_scan_eco_element_info);
        plantDiversityInfo = root.findViewById(R.id.image_view_eco_scan_plant_diversity_info);
        floweringTimeInfo = root.findViewById(R.id.image_view_eco_scan_flowering_time_info);

        //SeekBar
        landUseSeekBar = root.findViewById(R.id.seekbar_eco_scan_land_use);
        ecoElementSeekBar = root.findViewById(R.id.seekbar_eco_scan_eco_element);
        plantDiversitySeekBar = root.findViewById(R.id.seekbar_eco_scan_plant_diversity);

        //Button
        saveToCalendarBtn = root.findViewById(R.id.button_eco_scan_save_result_to_calendar);
        shareResultBtn = root.findViewById(R.id.button_eco_scan_share_result);

        //Chart
        combinedChart = root.findViewById(R.id.combined_chart_eco_scan);
        progressBar = root.findViewById(R.id.progressBar_eco_scan);

        //click event listeners
        saveToCalendarBtn.setOnClickListener(this);
        shareResultBtn.setOnClickListener(this);
        landUseInfoImg.setOnClickListener(this);
        ecoElementInfo.setOnClickListener(this);
        plantDiversityInfo.setOnClickListener(this);
        floweringTimeInfo.setOnClickListener(this);
        expandDescIcon.setOnClickListener(this);

        //EditText Change listener
        surfaceAreaEdtText.addTextChangedListener(this);
        greenAreaEdtText.addTextChangedListener(this);

    }

    private void setupInitialUI() {
        ApplicationUser user = PreferencesUtility.getUser(getContext());
        gardenOwnerTxt.setText(user != null ? user.getName() : "");

        ecoScanDateLabel.setText(TimeUtils.todayDate("dd-MM-yyyy"));

        surfaceAreaLabelTxt.setText("Außenfläche = Bepflanzte +\nversiegelte Außenfläche");
        greenAreaLabelTxt.setText("Begrünte Fläche");
    }

    private void restorePreviousState() {
        // restore surface area and green area from sharedPref
        String savedSurfaceArea = PreferencesUtility.getSurfaceArea(getContext());
        String savedGreenArea = PreferencesUtility.getGreenArea(getContext());
        surfaceAreaEdtText.setText(savedSurfaceArea);
        greenAreaEdtText.setText(savedGreenArea);
    }

    private void setupSeekBarGradients() {
        //default seekBar LinearGradient, evenly distributed colors
        getSeekBarGradientDrawable(DEFAULT_RED, DEFAULT_YELLOW_START, DEFAULT_YELLOW_END, DEFAULT_GREEN, landUseSeekBar);

        //default values for ecoscan and plantdiversity seekbar as described in documentation
        getSeekBarGradientDrawable(DEFAULT_ECO_ELEMENT_RED, DEFAULT_ECO_ELEMENT_YELLOW_START, DEFAULT_ECO_ELEMENT_YELLOW_END, DEFAULT_ECO_ELEMENT_GREEN, ecoElementSeekBar);
        getSeekBarGradientDrawable(DEFAULT_DIVERSITY_RED, DEFAULT_DIVERSITY_YELLOW_START, DEFAULT_DIVERSITY_YELLOW_END, DEFAULT_DIVERSITY_GREEN, plantDiversitySeekBar);

    }

    private void getDurationRatingFromApi() {
        String floweringPlantUrl = APP_URL.USER_PLANT_API + "durationRatingPlant";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(floweringPlantUrl, this::onSuccessDurationChart, this::onErrorChartData, DurationRatingPlant[].class, new RequestData(RequestType.DurationRatingPlant));
        progressBar.setVisibility(View.VISIBLE);
    }

    private void onErrorChartData(Exception e, RequestData requestData) {
        if (isVisible()) {
            displayAlertDialog(getContext(), "Fehler beim Abfragen der Daten (ChartData)");
            progressBar.setVisibility(View.GONE);
        }
    }


    private void onSuccessDurationChart(DurationRatingPlant[] model, RequestData data) {
        if (isVisible()) {
            Log.d(TAG, "onSuccessChartData: ");
            durationRatingPlantList = Arrays.asList(model);

            createCombinedChartData(durationRatingPlantList);
            //load plant rating
            String plantRating = APP_URL.USER_PLANT_API + "ratingPlant";

            RequestQueueSingleton.getInstance(getContext()).stringRequest(plantRating, Request.Method.GET, this::onSuccessRatingPlant, this::onFailureRatingPlant, null);
            progressBar.setVisibility(View.VISIBLE);
        }
    }

    private void onFailureRatingPlant(VolleyError volleyError) {
        displayAlertDialog(getContext(), "Fehler beim Bewerten der Pflanzen (VolleyError)");
    }

    private void onSuccessRatingPlant(String _plantRating) {
        if (isVisible()) {
            plantDiversitySeekBar.setProgress(Integer.parseInt(_plantRating));
            plantDiversitySeekBar.setEnabled(false);
            plantsRating = _plantRating;

            //load ecoPlant rating

            String ecoPlantRating = APP_URL.USER_GARDEN_API + "ratingTotalEcoEl" + isAndroid();
            RequestQueueSingleton.getInstance(getContext()).stringRequest(ecoPlantRating, Request.Method.GET, this::onSuccessEcoPlantRating, this::onFailureEcoPlantRating, null);
            progressBar.setVisibility(View.VISIBLE);
        }

    }

    private void onFailureEcoPlantRating(VolleyError volleyError) {
        displayAlertDialog(getContext(), "Fehler beim Bewerten des Ökoscans (VolleyError)");

    }

    private void onSuccessEcoPlantRating(String rating) {
        if (isVisible()) {
            progressBar.setVisibility(View.GONE);
            ecoElementSeekBar.setProgress(Integer.parseInt(rating));
            gardenRating = rating;
            ecoElementSeekBar.setEnabled(false);
        }

    }


    private void createCombinedChartData(List<DurationRatingPlant> durationRatingPlants) {

        plantCount = new Integer[durationRatingPlants.size()];
        for (int i = 0; i < durationRatingPlants.size(); i++) {
            plantCount[i] = durationRatingPlants.get(i).getPlantCount();
        }

        // combining charts
        updateChartData();

        hideChartGridLines();

        configureCombinedChartAppearance();

    }

    private void hideChartGridLines() {
        combinedChart.getAxisRight().setDrawGridLines(false);
        combinedChart.getAxisLeft().setDrawGridLines(false);
        combinedChart.getXAxis().setDrawGridLines(false);
        combinedChart.getAxisLeft().setAxisMinimum(0);
        combinedChart.getAxisRight().setAxisMinimum(0);
    }

    private void updateChartData() {

        lineData = generateLineData();
        barData = generateBarData();

        combinedData = new CombinedData();
        combinedData.clearValues();
        combinedData.setData(barData);
        combinedData.setData(lineData);

        combinedChart.setData(combinedData);

        combinedChart.notifyDataSetChanged();
        combinedChart.invalidate();

    }
    private LineData generateLineData() {

        LineData d = new LineData();

        ArrayList<Entry> lineEntries = new ArrayList<>();

        for (int i = 0; i < lineChartData.length; i++) {
            lineEntries.add(new BarEntry(i, lineChartData[i]));
        }

        LineDataSet set = new LineDataSet(lineEntries, "Optimum für deine Gartengröße");
        set.setColor(Color.rgb(0, 0, 0));
        set.setLineWidth(2f);
        set.setMode(LineDataSet.Mode.CUBIC_BEZIER);
        set.setDrawValues(false);
        set.setValueTextSize(0f);
        set.setDrawCircles(false);

        d.addDataSet(set);

        return d;
    }

    private BarData generateBarData() {
        BarData barData;
        ArrayList<BarEntry> barEntryList = new ArrayList<>();

        for (int i = 0; i < lineChartData.length; i++) {
            // generating stacked bar chart
            barEntryList = generateStackedBarChart(barEntryList, i);
        }

        BarDataSet set1;


        set1 = new BarDataSet(barEntryList, "");
        set1.setDrawValues(false);
        set1.setDrawIcons(false);
        set1.setColors(getBarChartColors(), getContext());
        ArrayList<IBarDataSet> dataSets = new ArrayList<>();
        dataSets.add(set1);

        barData = new BarData(dataSets);

        return barData;
    }

    private ArrayList<BarEntry> generateStackedBarChart(ArrayList<BarEntry> yVals1, int i) {
        float valOptimum = 0;
        float valOptimumExcess = 0;

        if (plantCount != null) {
            if (isNotExcessChart(i)) {
                valOptimum = plantCount[i];
                valOptimumExcess = 0; // excess bar chart
            } else {
                valOptimum = lineChartData[i];
                valOptimumExcess = calculateExcessBar(plantCount[i], valOptimum);
            }
        }
        yVals1.add(new BarEntry(i, new float[]{valOptimum, valOptimumExcess}));
        return yVals1;
    }

    private float calculateExcessBar(Integer integer, float valOptimum) {
        return integer - valOptimum;
    }

    private boolean isNotExcessChart(int i) {
        return lineChartData[i] > plantCount[i];
    }

    private int[] getBarChartColors() {
        return new int[]{R.color.barChart_ecoScan_darkLime, R.color.barChart_ecoScan_berlyGreen};
    }

    private void configureCombinedChartAppearance() {
        barData.setValueTextSize(12f);

        combinedChart.getDescription().setEnabled(false);
        combinedChart.setDrawValueAboveBar(false);
        combinedChart.getLegend().setEnabled(false);   // Hide the legend

        XAxis xAxis = combinedChart.getXAxis();
        xAxis.setValueFormatter(new ValueFormatter() {
            @Override
            public String getFormattedValue(float value) {
                return Months[(int) value];
            }
        });
        xAxis.setPosition(XAxis.XAxisPosition.BOTTOM);
        xAxis.setGranularity(1f);
        xAxis.setLabelCount(Months.length);
    }

    private void getSeekBarGradientDrawable(float redColorEnd, float yellowColorStart, float yellowColorEnd, float greenColorStart, SeekBar seekBar) {

        int[] colors = new int[]{getContext().getResources().getColor(R.color.seekbar_ecoScan_red, null),
                getContext().getResources().getColor(R.color.seekbar_ecoScan_red, null),
                getContext().getResources().getColor(R.color.seekbar_ecoScan_yellow, null),
                getContext().getResources().getColor(R.color.seekbar_ecoScan_yellow, null),
                getContext().getResources().getColor(R.color.seekbar_ecoScan_green, null),
                getContext().getResources().getColor(R.color.seekbar_ecoScan_green, null)};

        ShapeDrawable p = new ShapeDrawable();
        p.setShape(new RoundRectShape(new float[]{10, 10, 10, 10, 10, 10, 10, 10}, null, null));

        ShapeDrawable.ShaderFactory sf = new ShapeDrawable.ShaderFactory() {
            @Override
            public Shader resize(int width, int height) {
                LinearGradient lg = new LinearGradient(0, 0, seekBar.getWidth(), 0,
                        colors,
                        new float[]{
                                0f, redColorEnd, yellowColorStart, yellowColorEnd, greenColorStart, 1f},
                        Shader.TileMode.CLAMP);
                return lg;
            }
        };

        p.setShaderFactory(sf);
        seekBar.setProgressDrawable(p);
    }

    @Override
    public void onClick(View view) {
        String message;

        switch (view.getId()) {

            case R.id.button_eco_scan_save_result_to_calendar:

                String apiUrl = APP_URL.ACCOUNT_API + "userinfo/";
                RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccessUser, null, UserInfo.class, new RequestData(RequestType.UserInfo));
                progressBar.setVisibility(View.VISIBLE);

                break;

            case R.id.button_eco_scan_share_result:

                //change fragment
                Bitmap bitmap = createScreenshot(mainScrollView.getChildAt(0), mainScrollView.getChildAt(0).getHeight(), mainScrollView.getChildAt(0).getWidth());
                byte[] bytesArray = getImageBytes(bitmap);
                Bundle args = new Bundle();
                args.putByteArray(SCREENSHOT_ECOSCAN_KEY, bytesArray);
                navigateToFragment(R.id.nav_controller_eco_scan_share_result, (Activity) getContext(), false, args);

                break;

            case R.id.expand_icon_eco_scan_desc:
                updateDescriptionUiOnExpand();

                break;

            case R.id.image_view_eco_scan_land_use_info:

                message = "Je weniger Flächen versiegelt werden desto besser. Optimal ist es, wenn so viel Außenflächen, wie möglich aus offenem Boden mit Pflanzenbewuchs bestehen.";
                displayInfoDropDownMenu(getContext(), view, message);
                break;
            case R.id.image_view_eco_scan_eco_element_info:

                message = "Insektenhotel, Nistkästen oder Komposthaufen - je mehr Ökokriterien im Garten realisiert werden, desto ökologischer ist dein Garten.";
                displayInfoDropDownMenu(getContext(), view, message);

                break;
            case R.id.image_view_eco_scan_plant_diversity_info:

                message = "Artenvielfalt ist das A und O für ein stabiles ökologisches Gleichgewicht im Garten. Je mehr verschiedene sinnvolle Pflanzen kultiviert werden, desto besser.";
                displayInfoDropDownMenu(getContext(), view, message);
                break;
            case R.id.image_view_eco_scan_flowering_time_info:

                message = "Menge der insektenfreundlichen, blühenden Pflanzen nach Monaten";
                displayInfoDropDownMenu(getContext(), view, message);
                break;

        }

    }

    private void updateDescriptionUiOnExpand() {
        if (!isExpandDesc) {
            expandDescIcon.setImageResource(R.drawable.collapse);
            ecoScanAboutDescLabel.setMaxLines(50);
            isExpandDesc = true;
        } else {
            expandDescIcon.setImageResource(R.drawable.expand);
            ecoScanAboutDescLabel.setMaxLines(4);
            isExpandDesc = false;
        }
    }

    private void onSuccessUser(UserInfo userInfo, RequestData requestData) {
        try {

            JSONObject mainObject = new JSONObject();
            ApplicationUser user = PreferencesUtility.getUser(getContext());
            UserMainGarden userMainGarden = PreferencesUtility.getUserMainGarden(getContext());

            JSONObject descriptionObject = setDescriptionAndUserData(userInfo);
            String base64_DescriptionJson = Base64.encodeToString(descriptionObject.toString().getBytes("utf-8"), Base64.DEFAULT);

            // if in development use hardcoded email password
            mainObject.put("Title", "Mein Ökoscan Android");
            mainObject.put("Description", base64_DescriptionJson);
            mainObject.put("Date", TimeUtils.todayDate("yyyy-MM-dd"));
            mainObject.put("UserId", user.getUserId());
            mainObject.put("EntryObjectId", userMainGarden.getId());
            mainObject.put("EntryOf", "20");

            String saveToDiaryUrl = APP_URL.DIARY_API;

            RequestQueueSingleton.getInstance(getContext()).objectRequest(saveToDiaryUrl, Request.Method.POST, this::SaveToCalendarSuccess, null, mainObject);


        } catch (Exception e) {
            Toast.makeText(getContext(), e.toString(), Toast.LENGTH_LONG).show();
        }
    }

    @NotNull
    private JSONObject setDescriptionAndUserData(UserInfo userInfo) throws JSONException {
        JSONObject descriptionObject = new JSONObject();
        JSONObject userInfoObject = new JSONObject();

        descriptionObject.put("gardenRating", gardenRating);
        descriptionObject.put("areaRating", String.valueOf(areaRating));
        descriptionObject.put("plantsRating", plantsRating);
        descriptionObject.put("totalArea", String.valueOf(landArea));
        descriptionObject.put("gardenArea", String.valueOf(greenArea));
        descriptionObject.put("graph", Arrays.toString(plantCount));
        descriptionObject.put("label", Arrays.toString(Months));
        descriptionObject.put("date", TimeUtils.todayDate("yyyy-MM-dd"));

        //user info object params
        userInfoObject.put("HouseNr", String.valueOf(userInfo.getHouseNr()));
        userInfoObject.put("FirstName", userInfo.getFirstName());
        userInfoObject.put("LastName", userInfo.getLastName());
        userInfoObject.put("UserName", userInfo.getUserName());
        userInfoObject.put("City", userInfo.getCity());
        userInfoObject.put("Zip", userInfo.getZip());
        userInfoObject.put("Country", userInfo.getCountry());

        descriptionObject.put("userInfo", userInfo);
        return descriptionObject;
    }

    private void SaveToCalendarSuccess(JSONObject jsonObject) {
        if (jsonObject != null) {
            Toast.makeText(getContext(), "Eintrag wurde erstellt", Toast.LENGTH_SHORT).show();
            progressBar.setVisibility(View.GONE);
        }
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
        calculateSurfaceArea();
    }

    @Override
    public void afterTextChanged(Editable editable) {
    }

    private void calculateSurfaceArea() {
        // if getText() value is greater than 0 get the value, otherwise assign value to 0
        landArea = !TextUtils.isEmpty(surfaceAreaEdtText.getText()) ? Double.parseDouble(surfaceAreaEdtText.getText().toString()) : 0;
        greenArea = !TextUtils.isEmpty(greenAreaEdtText.getText()) ? Double.parseDouble(greenAreaEdtText.getText().toString()) : 0;

        // save to shared preferences
        PreferencesUtility.setSurfaceArea(getContext(), String.valueOf(landArea));
        PreferencesUtility.setGreenArea(getContext(), String.valueOf(greenArea));

        // configure gradient area
        if (isLandAreaFrom0To99()) {
            configureDynamicLandAreaGradient(CATEGORY_1);
            lineChartData = new Integer[]{2, 4, 6, 7, 15, 20, 20, 15, 10, 6, 4, 2};

        } else if (isLandAreaFrom100To499()) {
            configureDynamicLandAreaGradient(CATEGORY_2);

            lineChartData = new Integer[]{4, 6, 10, 13, 20, 30, 30, 20, 15, 8, 5, 4};

        } else if (isLandAreaFrom500To999()) {
            configureDynamicLandAreaGradient(CATEGORY_3);
            lineChartData = new Integer[]{5, 7, 12, 16, 27, 40, 40, 27, 20, 12, 7, 5};

        } else if (isLandAreaGreaterThan999()) {
            configureDynamicLandAreaGradient(CATEGORY_4);
            lineChartData = new Integer[]{6, 8, 15, 25, 35, 50, 50, 35, 25, 14, 8, 6};
        }

        landUseArea = calculateArea();
        landUseSeekBar.setProgress((int) landUseArea);

        // clear lineChart values and repopulate
        updateChartData();

    }

    private double calculateArea() {
        if (landArea > 0 && greenArea > 0) {
            landUseArea = (greenArea / landArea) * 100;
            areaRating = (int) ((landArea / landArea) * 100);
        }
        return landUseArea;
    }

    private boolean isLandAreaGreaterThan999() {
        return landArea >= 1000;
    }

    private boolean isLandAreaFrom500To999() {
        return landArea >= 500 && landArea < 999;
    }

    private boolean isLandAreaFrom100To499() {
        return landArea >= 100 && landArea <= 499;
    }

    private boolean isLandAreaFrom0To99() {
        return landArea >= 0 && landArea < 100;
    }

    /**
     * Scale from red to green
     * - Kategorie 1: 0% - 9% rot 10% - 32% gelb 33% - 100% grün
     * - Kategorie 2: 0% - 33% rot 34% - 59% gelb 60% - 100% grün
     * - Kategorie 3: 0% - 50% rot 51% - 69% gelb 70% - 100% grün
     * - Kategorie 4: 0% - 60% rot 61% - 79% gelb 80% - 100% grün
     * Note: between gradient there is gaps of few points for gradient transition
     *
     * @param categoryId: based on categoryId seekbar gradient is updated
     */

    private void configureDynamicLandAreaGradient(int categoryId) {
        switch (categoryId) {
            case CATEGORY_1:
                getSeekBarGradientDrawable(CATEGORY_1_RED, CATEGORY_1_YELLOW_START, CATEGORY_1_YELLOW_END, CATEGORY_1_GREEN, landUseSeekBar);
                break;
            case CATEGORY_2:
                getSeekBarGradientDrawable(CATEGORY_2_RED, CATEGORY_2_YELLOW_START, CATEGORY_2_YELLOW_END, CATEGORY_2_GREEN, landUseSeekBar);
                break;
            case CATEGORY_3:
                getSeekBarGradientDrawable(CATEGORY_3_RED, CATEGORY_3_YELLOW_START, CATEGORY_3_YELLOW_END, CATEGORY_3_GREEN, landUseSeekBar);
                break;
            case CATEGORY_4:
                getSeekBarGradientDrawable(CATEGORY_4_RED, CATEGORY_4_YELLOW_START, CATEGORY_4_YELLOW_END, CATEGORY_4_GREEN, landUseSeekBar);
                break;
            default:
                getSeekBarGradientDrawable(DEFAULT_RED, DEFAULT_YELLOW_START, DEFAULT_YELLOW_END, DEFAULT_GREEN, landUseSeekBar);
        }
    }

    //create bitmap from the ScrollView
    private Bitmap createScreenshot(View view, int height, int width) {
        Bitmap bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);
        Canvas canvas = new Canvas(bitmap);
        Drawable bgDrawable = view.getBackground();
        if (bgDrawable != null)
            bgDrawable.draw(canvas);
        else
            canvas.drawColor(Color.WHITE);
        view.draw(canvas);

        Bitmap scaledBitmap = resizeBitmap(bitmap);

        return scaledBitmap;
    }

    private Bitmap resizeBitmap(Bitmap bitmap) {
        int newWidth = (bitmap.getWidth() * BITMAP_A4_PAGE_HEIGHT) / bitmap.getHeight();
        return Bitmap.createScaledBitmap(bitmap, newWidth, BITMAP_A4_PAGE_HEIGHT, false);
    }

    /**
     * Check if user entered info (either by authenticating or by entering the data manually)
     * exists. If it doesn't, redirect to LoginFragment.
     */
    @Override
    public void onResume() {
        super.onResume();
        authenticationCheck();
    }

    private void authenticationCheck() {
        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login,(Activity) getContext(), true, null);
        }
    }

}