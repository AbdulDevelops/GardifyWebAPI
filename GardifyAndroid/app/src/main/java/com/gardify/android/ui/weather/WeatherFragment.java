package com.gardify.android.ui.weather;

import android.app.Activity;
import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.weather.DailyWeather;
import com.gardify.android.data.weather.Forecast;
import com.gardify.android.data.weather.TodayWeather;
import com.gardify.android.generic.RecycleViewGenericAdapter;
import com.gardify.android.ui.generic.CarouselGroup;
import com.gardify.android.ui.generic.ExpandableNoBgHeaderItem;
import com.gardify.android.ui.generic.decoration.CarouselItemDecoration;
import com.gardify.android.ui.generic.recyclerItem.CardViewTopBottomSection;
import com.gardify.android.ui.generic.recyclerItem.HeaderTitle;
import com.gardify.android.ui.weather.recyclerItems.DailyWeatherCarouselItem;
import com.gardify.android.ui.weather.recyclerItems.HourlyWeatherCardItem;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.gardify.android.utils.TimeUtils;
import com.github.mikephil.charting.charts.BarChart;
import com.github.mikephil.charting.charts.CombinedChart;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.components.YAxis;
import com.github.mikephil.charting.data.BarData;
import com.github.mikephil.charting.data.BarDataSet;
import com.github.mikephil.charting.data.BarEntry;
import com.github.mikephil.charting.data.CombinedData;
import com.github.mikephil.charting.data.Entry;
import com.github.mikephil.charting.data.LineData;
import com.github.mikephil.charting.data.LineDataSet;
import com.github.mikephil.charting.formatter.ValueFormatter;
import com.github.mikephil.charting.interfaces.datasets.IBarDataSet;
import com.github.mikephil.charting.interfaces.datasets.ILineDataSet;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.Group;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import org.jetbrains.annotations.NotNull;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.utils.APP_URL.isAndroid;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;
import static java.util.Calendar.HOUR_OF_DAY;
import static java.util.Calendar.getInstance;

;

public class WeatherFragment extends Fragment implements RecycleViewGenericAdapter.OnItemClickListener<Forecast> {

    public static final String API_DATE_PATTERN = "yyyy-MM-dd'T'HH:00:00XXX";
    public static final String TODAY_WEATHER_FORECAST = "PT0S";
    public static final String PRECIPITATION_WEATHER_FORECAST = "PT1H";
    public static final String HOUR_DATE_PATTERN = "HH";
    public static final int HOUR_6_AM = 06;
    public static final int HOUR_7_AM = 07;
    public static final int HOUR_10_AM = 10;
    public static final int HOUR_11_AM = 11;
    public static final int HOUR_3_PM = 15;
    public static final int HOUR_4_PM = 16;
    public static final int HOUR_7_PM = 19;
    public static final int HOUR_8_PM = 20;
    public static final int HOUR_11_PM = 23;
    public static final int HOUR_12_AM = 00;
    public static final int PADDING_1 = 1;
    public static final int TEMPERATURE_0_CELSIUS = 0;
    public static final int PADDING_2 = 2;
    public static final int MAX_SUN_SHINE = 16;
    private int currentHour;
    private List<Forecast> forecastsTodayWeather;
    private List<Forecast> forecastsPrecipitation; // contains precipitation
    //Groupie adapter
    private GroupAdapter groupAdapter;
    private GridLayoutManager layoutManager;
    private RecyclerView hourlyForecastRecycleView;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_weather, container, false);
        //setup Toolbar
        setupToolbar(getActivity(), "GARTEN WETTER", R.drawable.gardify_app_icon_gartenwetter, R.color.toolbar_gardenWeather_setupToolbar, true);

        //initializing views
        initializeViews(root);
        setupGroupAdapter();

        String apiUrl = APP_URL.Forecast_URL + "todayForecast";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccess, this::onError, TodayWeather.class, new RequestData(RequestType.TodayWeather));

        return root;
    }

    private TextView cityLabelTxt, dateLabelTxt, celsiusLabelTxt, precipitationLabelText,
            airPressureLabelTxt, humidityLabelTxt, windDirectionSpeedLabelTxt, weatherClarityTxt;
    private ImageView weatherIcon;
    private ProgressBar progressBar;
    private BarChart sunshineChart;
    private CombinedChart combinedTemperatureRainfallChart;

    public void initializeViews(View root) {
        /* finding views block */
        cityLabelTxt = root.findViewById(R.id.textView_weather_city_label);
        dateLabelTxt = root.findViewById(R.id.textView_weather_date_label);
        celsiusLabelTxt = root.findViewById(R.id.textView_weather_celsius_label);
        precipitationLabelText = root.findViewById(R.id.text_view_weather_precipitation);
        windDirectionSpeedLabelTxt = root.findViewById(R.id.text_view_weather_wind_direction_speed);
        airPressureLabelTxt = root.findViewById(R.id.text_view_weather_air_pressure);
        humidityLabelTxt = root.findViewById(R.id.text_view_weather_humidity);
        weatherClarityTxt = root.findViewById(R.id.textView_weather_clarity_label);
        weatherIcon = root.findViewById(R.id.imageView_weather_icon);
        progressBar = root.findViewById(R.id.progressBar_weather);
        sunshineChart = root.findViewById(R.id.bar_chart_weather_sunshine);

        hourlyForecastRecycleView = root.findViewById(R.id.recycleView_weather_hourly_forecast);
        combinedTemperatureRainfallChart = root.findViewById(R.id.combined_chart_weather);
    }

    private void setupGroupAdapter() {
        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        hourlyForecastRecycleView.setLayoutManager(layoutManager);
        populateAdapter();
        hourlyForecastRecycleView.setAdapter(groupAdapter);
    }

    private ExpandableGroup expandableGroupHourlyMorning, expandableGroupHourlyNoon, expandableGroupHourlyEvening,
            expandableGroupHourlyNight;

    private Group carousel;

    private void populateAdapter() {

        // section with background
        Section cardViewTopSection = new Section(new CardViewTopBottomSection(true));
        Section cardViewBottomSection = new Section(new CardViewTopBottomSection(false));

        Section sectionMorning = setSectionHeaderFooter(cardViewTopSection, cardViewBottomSection);
        Section sectionNoon = setSectionHeaderFooter(cardViewTopSection, cardViewBottomSection);
        Section sectionEvening = setSectionHeaderFooter(cardViewTopSection, cardViewBottomSection);
        Section sectionNight = setSectionHeaderFooter(cardViewTopSection, cardViewBottomSection);

        // Expandable group morning
        ExpandableNoBgHeaderItem expandableHeaderMorning = new ExpandableNoBgHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.gardenWeather_morning);
        expandableGroupHourlyMorning = new ExpandableGroup(expandableHeaderMorning);
        sectionMorning.add(expandableGroupHourlyMorning);

        // Expandable group noon
        ExpandableNoBgHeaderItem expandableHeaderNoon = new ExpandableNoBgHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.gardenWeather_noon);
        expandableGroupHourlyNoon = new ExpandableGroup(expandableHeaderNoon);
        sectionNoon.add(expandableGroupHourlyNoon);

        // Expandable group evening
        ExpandableNoBgHeaderItem expandableHeaderEvening = new ExpandableNoBgHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.gardenWeather_evening);
        expandableGroupHourlyEvening = new ExpandableGroup(expandableHeaderEvening);
        sectionEvening.add(expandableGroupHourlyEvening);

        // Expandable group night
        ExpandableNoBgHeaderItem expandableHeaderNight = new ExpandableNoBgHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.gardenWeather_night);
        expandableGroupHourlyNight = new ExpandableGroup(expandableHeaderNight);
        sectionNight.add(expandableGroupHourlyNight);

        groupAdapter.add(new HeaderTitle(R.string.gardenWeather_hourlyForecast));
        groupAdapter.add(sectionMorning);
        groupAdapter.add(sectionNoon);
        groupAdapter.add(sectionEvening);
        groupAdapter.add(sectionNight);

        groupAdapter.add(new HeaderTitle(R.string.gardenWeather_outlook));

    }

    private Section setSectionHeaderFooter(Section cardViewTopSection, Section cardViewBottomSection) {
        Section section = new Section();
        section.setHeader(cardViewTopSection);
        section.setFooter(cardViewBottomSection);

        return section;
    }


    private void onSuccess(TodayWeather model, RequestData data) {
        TodayWeather todayWeatherList;

        todayWeatherList = model;

        currentHour = getInstance().get(HOUR_OF_DAY);

        forecastsTodayWeather = todayWeatherList.getForecasts().stream().filter(p -> p.getValidPeriod().equalsIgnoreCase(TODAY_WEATHER_FORECAST))
                .collect(Collectors.toList());

        showCurrentHourWeather(forecastsTodayWeather);

        forecastsPrecipitation = todayWeatherList.getForecasts().stream().filter(p -> p.getValidPeriod().equalsIgnoreCase(PRECIPITATION_WEATHER_FORECAST))
                .collect(Collectors.toList());

        showCurrentHourPrecipitation(forecastsPrecipitation);

        generateHourlyForecast(forecastsTodayWeather);

        //get daily forecast data
        String apiUrl = APP_URL.Forecast_URL + "daily" + isAndroid();
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccessDaily, this::onError, DailyWeather.class, new RequestData(RequestType.DailyWeather));


    }

    private void showCurrentHourWeather(List<Forecast> forecastsTodayWeather) {
        for (Forecast forecast : forecastsTodayWeather) {
            String forecastHour = TimeUtils.dateToString(forecast.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN);
            String currentHourFormattedTwoDigit = String.format("%02d", currentHour);
            if (forecastHour.equals(currentHourFormattedTwoDigit)) {

                dateLabelTxt.setText("Aktuell, " + TimeUtils.dateToString(forecast.getValidFrom(), API_DATE_PATTERN, "dd.MM.yyyy"));
                celsiusLabelTxt.setText(String.format(", %s °C", Math.round(forecast.getAirTemperatureInCelsius())));

                //today weather detail
                String windStrength = setWindStrength(forecast.getWindSpeedInBeaufort());
                String windOrigin = setWindOrigin(forecast.getWindDirectionInDegree());
                String windSpeed = forecast.getWindSpeedInKilometerPerHour() + "km/h";

                windDirectionSpeedLabelTxt.setText(String.format("%s aus %s (%s)", windStrength, windOrigin, windSpeed));
                airPressureLabelTxt.setText(String.format("Luftdruck: %s hPa", forecast.getAirPressureAtSeaLevelInHectoPascal()));
                humidityLabelTxt.setText(forecast.getRelativeHumidityInPercent() + " % Luftfeuchte");

            }

        }
    }

    private void showCurrentHourPrecipitation(List<Forecast> forecastsPrecipitation) {
        for (Forecast forecast : forecastsPrecipitation) {
            String forecastHour = TimeUtils.dateToString(forecast.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN);
            String currentHourFormattedTwoDigit = String.format("%02d", currentHour);
            if (forecastHour.equals(currentHourFormattedTwoDigit)) {
                // weather clarity
                String weatherClarity = getWeatherClarity(forecast, forecast.getPrecipitationAmountInMillimeter(), forecastsPrecipitation, weatherIcon);
                weatherClarityTxt.setText(weatherClarity);

                String precipitationPercent = String.valueOf(forecast.getPrecipitationAmountInMillimeter() != null ? forecast.getPrecipitationAmountInMillimeter() : "0,0");
                String precipitation = forecast.getPrecipitationProbabilityInPercent() + "% /" + precipitationPercent + "l/m\u00B2";
                precipitationLabelText.setText(precipitation);

            }

        }
    }

    private void generateHourlyForecast(List<Forecast> forecasts_pt0S) {

        //forecasts_pt0S = takeIncomingHoursOnly(forecasts_pt0S);

        List<Forecast> morningWeatherDetails = forecasts_pt0S.stream().filter(p -> Integer.parseInt(TimeUtils.dateToString(p.getValidFrom(),
                API_DATE_PATTERN, HOUR_DATE_PATTERN)) > HOUR_6_AM && Integer.parseInt(TimeUtils.dateToString(p.getValidFrom(),
                API_DATE_PATTERN, HOUR_DATE_PATTERN)) < HOUR_10_AM)
                .collect(Collectors.toList());

        List<Forecast> noonWeatherDetails = forecasts_pt0S.stream().filter(p -> Integer.parseInt(TimeUtils.dateToString(p.getValidFrom(),
                API_DATE_PATTERN, HOUR_DATE_PATTERN)) >= HOUR_10_AM && Integer.parseInt(TimeUtils.dateToString(p.getValidFrom(),
                API_DATE_PATTERN, HOUR_DATE_PATTERN)) < HOUR_3_PM)
                .collect(Collectors.toList());

        List<Forecast> eveningWeatherDetails = forecasts_pt0S.stream().filter(p -> Integer.parseInt(TimeUtils.dateToString(p.getValidFrom(),
                API_DATE_PATTERN, HOUR_DATE_PATTERN)) >= HOUR_3_PM && Integer.parseInt(TimeUtils.dateToString(p.getValidFrom(),
                API_DATE_PATTERN, HOUR_DATE_PATTERN)) <= HOUR_7_PM)
                .collect(Collectors.toList());

        List<Forecast> nightWeatherDetails = forecasts_pt0S.stream().filter(p -> Integer.parseInt(TimeUtils.dateToString(p.getValidFrom(),
                API_DATE_PATTERN, HOUR_DATE_PATTERN)) >= HOUR_8_PM && Integer.parseInt(TimeUtils.dateToString(p.getValidFrom(),
                API_DATE_PATTERN, HOUR_DATE_PATTERN)) <= HOUR_11_PM)
                .collect(Collectors.toList());

        for (Forecast forecast : morningWeatherDetails) {
            expandableGroupHourlyMorning.add(new HourlyWeatherCardItem(forecast, forecastsPrecipitation));
        }
        for (Forecast forecast : noonWeatherDetails) {
            expandableGroupHourlyNoon.add(new HourlyWeatherCardItem(forecast, forecastsPrecipitation));
        }
        for (Forecast forecast : eveningWeatherDetails) {
            expandableGroupHourlyEvening.add(new HourlyWeatherCardItem(forecast, forecastsPrecipitation));
        }
        for (Forecast forecast : nightWeatherDetails) {
            expandableGroupHourlyNight.add(new HourlyWeatherCardItem(forecast, forecastsPrecipitation));
        }

    }

    @NotNull
    private List<Forecast> takeIncomingHoursOnly(List<Forecast> forecasts_pt0S) {
        return forecasts_pt0S.stream().filter(p -> Integer.parseInt(TimeUtils.dateToString(p.getValidFrom(),
                API_DATE_PATTERN, HOUR_DATE_PATTERN)) >= currentHour ).collect(Collectors.toList());
    }

    String[] DAYS;
    String[] DATES;

    private void onSuccessDaily(DailyWeather model, RequestData data) {
        List<Forecast> forecastsList;

        forecastsList = model.getForecasts();

        cityLabelTxt.setText(model.getCity());
        DAYS = new String[forecastsList.size()];
        DATES = new String[forecastsList.size()];

        for (int i = 0; i < forecastsList.size(); i++) {
            DATES[i] = TimeUtils.dateToString(forecastsList.get(i).getValidFrom(), API_DATE_PATTERN, "dd.MM");
            DAYS[i] = TimeUtils.dateToString(forecastsList.get(i).getValidFrom(), API_DATE_PATTERN, "EE");
        }

        reloadDailyWeatherCarousel(forecastsList);

        //chart sunshine
        generateSunshineChartData(forecastsList);
        configureSunshineChartAppearance();

        //chart combined
        createCombinedChartData(forecastsList);
        configureCombinedChartAppearance();

        progressBar.setVisibility(View.GONE);
    }

    private void reloadDailyWeatherCarousel(List<Forecast> forecastsList) {
        if(carousel!=null){
            groupAdapter.remove(carousel);
        }
        carousel = makeEcoElementsCarouselGroup(forecastsList);
        groupAdapter.add(carousel);
        groupAdapter.add(new HeaderTitle(R.string.gardenWeather_eightDayTrend));

    }

    private Group makeEcoElementsCarouselGroup(List<Forecast> forecastsList) {
        int betweenPadding = getContext().getResources().getDimensionPixelSize(R.dimen.marginPaddingSize_8sdp);
        CarouselItemDecoration carouselDecoration = new CarouselItemDecoration(0, betweenPadding);
        GroupAdapter carouselAdapter = new GroupAdapter();
        for (Forecast dailyForecast : forecastsList) {
            carouselAdapter.add(new DailyWeatherCarouselItem(getContext(), dailyForecast));
        }
        return new CarouselGroup(carouselDecoration, carouselAdapter);
    }


    public void onError(Exception ex, RequestData data) {
        if (getActivity() != null) {
            Resources res = getContext().getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());
            progressBar.setVisibility(View.GONE);
        }
    }

    public static String setWindStrength(int windSpeedInBeaufort) {
        String windStrength = " ";
        if (windSpeedInBeaufort == 0) {
            windStrength = "Windstille";
        } else if (windSpeedInBeaufort == 1) {
            windStrength = "Leiser Zug";
        } else if (windSpeedInBeaufort == 2) {
            windStrength = "leichte Brise";
        } else if (windSpeedInBeaufort == 3) {
            windStrength = "Schwache Brise";
        } else if (windSpeedInBeaufort == 4) {
            windStrength = "Mäßige Brise";
        } else if (windSpeedInBeaufort == 5) {
            windStrength = "Frische Brise";
        } else if (windSpeedInBeaufort == 6) {
            windStrength = "Starker Wind";
        } else if (windSpeedInBeaufort == 7) {
            windStrength = "Steifer Wind";
        } else if (windSpeedInBeaufort == 8) {
            windStrength = "Stürmischer Wind";
        } else if (windSpeedInBeaufort == 9) {
            windStrength = "Sturm";
        } else if (windSpeedInBeaufort == 10) {
            windStrength = "Schwerer Sturm";
        } else if (windSpeedInBeaufort == 11) {
            windStrength = "Orkanartiger Sturm";
        } else if (windSpeedInBeaufort == 12) {
            windStrength = "Orkan";
        }
        return windStrength;
    }

    public static String setWindOrigin(int windDirection) {

        String windOrigin = "";
        if (windDirection >= 0 && windDirection <= 45) {
            windOrigin = "N";
        } else if (windDirection > 45 && windDirection <= 90) {
            windOrigin = "O";
        } else if (windDirection > 90 && windDirection <= 135) {
            windOrigin = "SO";
        } else if (windDirection > 135 && windDirection <= 180) {
            windOrigin = "S";
        } else if (windDirection > 180 && windDirection <= 225) {
            windOrigin = "SW";
        } else if (windDirection > 225 && windDirection <= 270) {
            windOrigin = "W";
        } else if (windDirection > 270 && windDirection <= 315) {
            windOrigin = "NW";
        } else if (windDirection > 315 && windDirection <= 360) {
            windOrigin = "N";
        }
        return windOrigin;
    }


    public String getCityNameFromZip(String postalCode) {

        String zipCode = postalCode.substring(postalCode.length() - 5);

        return zipCode;

    }

    @Override
    public void onItemClick(Forecast position) {

    }

    double getMax(double a, double b) {
        if (a > b) {
            return a;
        } else {
            return b;
        }
    }

    int getCloudCoverLevel(Double cloudCoverLevel) {
        if (cloudCoverLevel < 1.5) {
            return 0;
        } else if (cloudCoverLevel >= 1.5 && cloudCoverLevel <= 5.0) {
            return 1;
        } else {
            return 2;
        }
    }

    int getPrecipitationLevel(double precipitationAmount, double rangeMax) {

        if (rangeMax > 0) {
            double maxPrecipitationAmount = getMax(precipitationAmount, rangeMax);

            if (maxPrecipitationAmount <= 1) {
                return 0;
            }
            if (1 < maxPrecipitationAmount && maxPrecipitationAmount <= 5) {
                return 1;
            }
        }
        return 2;
    }

    int windLevel(int value) {
        if (value < 5) {
            return 0;
        } else if (value >= 5 && value <= 6) {
            return 1;
        } else {
            return 2;
        }
    }

    Boolean isDayLight(String timeS) {
        int time = Integer.parseInt(timeS);
        return time > 06 && time <= 19;
    }

    public String getWeatherClarity(Forecast psoWeather, double pshPrecipitation, List<Forecast> forecasts_PT1H, ImageView weatherIcon) {

        String weatherClarity = "";

        double precipitationAmount = getMax(pshPrecipitation, psoWeather.getPrecipitationAmountInMillimeter());

        double rangeMax = forecasts_PT1H.stream().mapToDouble(s->s.getPrecipitationAmountInMillimeter()).sum();

        /*var rangeMax = range.reduce(into:Float()){
            output, input in

            var precipitationValues = input.forcast ?.map({$0.PrecipitationAmountInMillimeter!})
            var sum = 0.0
            precipitationValues ?.forEach {
                data in
                sum += Double(data !)
            }
            output += Float(sum)

        }*/

        // get max values for Precipitation



        if (getPrecipitationLevel(precipitationAmount, rangeMax) == 1) {
            switch (windLevel(psoWeather.getWindSpeedInBeaufort())) {
                case 0:
                    weatherClarity = "leichter Regen";
                    if (isDayLight(TimeUtils.dateToString(psoWeather.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN))) {
                        weatherIcon.setImageResource(R.mipmap.sonne_mit_regen_stufe_1);

                    } else {
                        weatherIcon.setImageResource(R.mipmap.mond_mit_regen_stufe_1);
                    }
                    break;

                case 1:
                    weatherClarity = "leichter Regen, Wind";
                    weatherIcon.setImageResource(R.mipmap.wind_mit_regen_stufe_1);
                    break;

                case 2:
                    weatherClarity = "starker Wind, Regen";
                    weatherIcon.setImageResource(R.mipmap.wind_mit_regen_stufe_2);
                    break;

                default:
                    break;
            }
        } else if (getPrecipitationLevel(precipitationAmount, rangeMax) == 1) {
            switch (windLevel(psoWeather.getWindSpeedInBeaufort())) {
                case 0:
                    weatherClarity = "starker Regen";
                    if (isDayLight(TimeUtils.dateToString(psoWeather.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN))) {
                        weatherIcon.setImageResource(R.mipmap.sonne_mit_regen_stufe_2);

                    } else {
                        weatherIcon.setImageResource(R.mipmap.mond_mit_regen_stufe_2);

                    }
                    break;

                case 1:
                    weatherClarity = "starker Regen, Wind";
                    weatherIcon.setImageResource(R.mipmap.wind_mit_regen_stufe_1);
                    break;

                case 2:
                    weatherClarity = "starker Wind, Regen";
                    weatherIcon.setImageResource(R.mipmap.wind_mit_regen_stufe_2);
                    break;

                default:
                    break;
            }
        } else if (getCloudCoverLevel(psoWeather.getCloudCoverLowerThan2000MeterInOcta()) == 0) {
            switch (windLevel(psoWeather.getWindSpeedInBeaufort())) {
                case 0:
                    weatherClarity = "klar";
                    if (isDayLight(TimeUtils.dateToString(psoWeather.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN))) {
                        weatherIcon.setImageResource(R.mipmap.sonne);

                    } else {
                        weatherIcon.setImageResource(R.mipmap.mond);
                    }
                    break;

                case 1:
                    weatherClarity = "leichter Wind";
                    if (isDayLight(TimeUtils.dateToString(psoWeather.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN))) {
                        weatherIcon.setImageResource(R.mipmap.wind_mit_sonne_stufe_1);

                    } else {
                        weatherIcon.setImageResource(R.mipmap.wind_mit_mond_stufe_1);
                    }
                    break;

                case 2:
                    weatherClarity = "starker Wind";
                    if (isDayLight(TimeUtils.dateToString(psoWeather.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN))) {
                        weatherIcon.setImageResource(R.mipmap.wind_mit_sonne_stufe_2);

                    } else {
                        weatherIcon.setImageResource(R.mipmap.wind_mit_mond_stufe_2);
                    }
                default:
                    break;
            }

        } else if (getCloudCoverLevel(psoWeather.getCloudCoverLowerThan2000MeterInOcta()) == 1) {
            if (getPrecipitationLevel(precipitationAmount, rangeMax) == 0) {
                switch (windLevel(psoWeather.getWindSpeedInBeaufort())) {
                    case 0:
                        weatherClarity = "leicht bewölkt";
                        if (isDayLight(TimeUtils.dateToString(psoWeather.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN))) {
                            weatherIcon.setImageResource(R.mipmap.wolke_mit_sonne);
                        } else {
                            weatherIcon.setImageResource(R.mipmap.wolke_mit_mond);
                        }
                        break;
                    case 1:
                        weatherClarity = "leicht bewölkt";
                        if (isDayLight(TimeUtils.dateToString(psoWeather.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN))) {
                            weatherIcon.setImageResource(R.mipmap.wind_mit_sonne_stufe_1);
                        } else {
                            weatherIcon.setImageResource(R.mipmap.wind_mit_mond_stufe_1);
                        }
                        break;
                    case 2:
                        weatherClarity = "starker Wind";
                        if (isDayLight(TimeUtils.dateToString(psoWeather.getValidFrom(), API_DATE_PATTERN, HOUR_DATE_PATTERN))) {
                            weatherIcon.setImageResource(R.mipmap.wind_mit_sonne_stufe_1);
                        } else {
                            weatherIcon.setImageResource(R.mipmap.wind_mit_sonne_stufe_1);
                        }
                    default:
                        break;
                }
            }

        } else if (getCloudCoverLevel(psoWeather.getCloudCoverLowerThan2000MeterInOcta()) == 2) {
            if (getPrecipitationLevel(precipitationAmount, rangeMax) == 0) {
                switch (windLevel(psoWeather.getWindSpeedInBeaufort())) {
                    case 0:
                        weatherClarity = "stark bewölkt";
                        weatherIcon.setImageResource(R.mipmap.wolken);
                        break;

                    case 1:
                        weatherClarity = "stark bewölkt, Wind";
                        weatherIcon.setImageResource(R.mipmap.wolken);
                        break;

                    case 2:
                        weatherClarity = "stark bewölkt & Wind";
                        weatherIcon.setImageResource(R.mipmap.wind_mit_regen_stufe_2);
                    default:
                        break;
                }

            } else if (getPrecipitationLevel(precipitationAmount, rangeMax) == 1) {
                switch (windLevel(psoWeather.getWindSpeedInBeaufort())) {
                    case 0:
                        weatherClarity = "Regen";
                        weatherIcon.setImageResource(R.mipmap.regen_stufe_1);
                        break;

                    case 1:
                        weatherClarity = "Regen & Wind";
                        weatherIcon.setImageResource(R.mipmap.wind_mit_regen_stufe_1);
                        break;

                    case 2:
                        weatherClarity = "starker Wind, Regen";
                        weatherIcon.setImageResource(R.mipmap.wind_mit_regen_stufe_2);
                        break;

                    default:
                        break;
                }

            } else if (getPrecipitationLevel(precipitationAmount, rangeMax) == 2) {
                switch (windLevel(psoWeather.getWindSpeedInBeaufort())) {
                    case 0:
                        weatherClarity = "starker Regen";
                        weatherIcon.setImageResource(R.mipmap.regen_stufe_2);
                        break;
                    case 1:
                        weatherClarity = "heftiger Regen";
                        weatherIcon.setImageResource(R.mipmap.wind_mit_gewitter_mit_regen_stufe_1);
                        break;
                    case 2:
                        weatherClarity = "stürmisch Regen";
                        weatherIcon.setImageResource(R.mipmap.wind_mit_gewitter_mit_regen_stufe_2);
                        break;

                    default:
                        break;
                }

            }

        }

        return weatherClarity;
    }


    private BarData generateSunshineChartData(List<Forecast> forecastsList) {
        ArrayList<BarEntry> values = new ArrayList<>();
        for (int i = 0; i < forecastsList.size(); i++) {
            values.add(new BarEntry(i, (float) (forecastsList.get(i).getSunshineDurationInMinutes() / 60)));
        }

        BarDataSet sunshineBarSet = new BarDataSet(values, "");
        sunshineBarSet.setDrawValues(false);
        int sunshineColor = ContextCompat.getColor(getContext(), R.color.view_gardenWeather_sunshine);
        sunshineBarSet.setColor(sunshineColor);

        ArrayList<IBarDataSet> dataSets = new ArrayList<>();
        dataSets.add(sunshineBarSet);

        BarData data = new BarData(dataSets);
        sunshineChart.setData(data);
        sunshineChart.invalidate();

        return data;
    }

    private void configureSunshineChartAppearance() {

        sunshineChart.getDescription().setEnabled(false);
        sunshineChart.setDrawValueAboveBar(false);

        XAxis xAxis = sunshineChart.getXAxis();
        xAxis.setValueFormatter(new ValueFormatter() {
            @Override
            public String getFormattedValue(float value) {
                return DATES[(int) value];
            }
        });

        xAxis.setPosition(XAxis.XAxisPosition.BOTTOM);

        YAxis axisLeft = sunshineChart.getAxisLeft();
        axisLeft.setGranularity(1f);
        axisLeft.setValueFormatter(new ValueFormatter() {
            @Override
            public String getFormattedValue(float value) {
                return String.format((int) value + "H");
            }
        });

        // Hide Background grid
        sunshineChart.getXAxis().setDrawGridLines(false);
        sunshineChart.getAxisLeft().setDrawGridLines(false);
        sunshineChart.getAxisRight().setDrawGridLines(false);
        // hide legends
        sunshineChart.getLegend().setEnabled(false);

        // hide right y Axis
        sunshineChart.getAxisRight().setDrawLabels(false);
        // padding
        sunshineChart.getAxisRight().setSpaceMax(0.5f);
        sunshineChart.getAxisLeft().setSpaceMax(0.5f);
        sunshineChart.getAxisLeft().setAxisMaximum(MAX_SUN_SHINE);

    }

    Float maxTemp, minTemp;
    Double maxPrecipitation, minPrecipitation;

    private void createCombinedChartData(List<Forecast> forecastsList) {

        // bar chart data
        BarDataSet precipitationDataSet = generatePrecipitationBarChart(forecastsList);

        ArrayList<IBarDataSet> dataSets = new ArrayList<>();
        dataSets.add(precipitationDataSet);
        BarData barChart = new BarData(dataSets);

        // line chart data
        LineDataSet minTempSet = generateMinTempLineChart(forecastsList);

        LineDataSet maxTempSet = generateMaxTempLineChart(forecastsList);

        List<ILineDataSet> lineDataSets = new ArrayList<>();
        lineDataSets.add(minTempSet);
        lineDataSets.add(maxTempSet);
        LineData lineData = new LineData(lineDataSets);

        // combining charts
        CombinedData combinedData = new CombinedData();
        combinedData.setData(barChart); // set BarChart...
        combinedData.setData(lineData);

        combinedTemperatureRainfallChart.setData(combinedData);
        combinedTemperatureRainfallChart.invalidate();

        // setting Temperature axis min max
        combinedTemperatureRainfallChart.getAxisLeft().setAxisMinimum(setMinTemperature());
        combinedTemperatureRainfallChart.getAxisLeft().setAxisMaximum(maxTemp + PADDING_2);

        combinedTemperatureRainfallChart.getAxisRight().setAxisMinimum(0);
        combinedTemperatureRainfallChart.getAxisRight().setAxisMaximum(maxPrecipitation.floatValue() + PADDING_2);

    }

    @NotNull
    private LineDataSet generateMaxTempLineChart(List<Forecast> forecastsList) {
        ArrayList<Entry> maxTempLineChartEntries = new ArrayList<>();

        for (int i = 0; i < forecastsList.size(); i++) {
            maxTempLineChartEntries.add(new BarEntry(i, forecastsList.get(i).getMaxAirTemperatureInCelsius().floatValue()));
        }

        // get max values for temperature
        maxTemp = Collections.max(maxTempLineChartEntries, Comparator.comparing(s -> s.getY())).getY();

        LineDataSet maxTempSet = new LineDataSet(maxTempLineChartEntries, "");
        YAxis leftAxis = combinedTemperatureRainfallChart.getAxisLeft();
        maxTempSet.setAxisDependency(leftAxis.getAxisDependency());
        maxTempSet.setMode(LineDataSet.Mode.CUBIC_BEZIER); // for smooth line chart
        maxTempSet.setLineWidth(2);
        int maxTemperatureColor = ContextCompat.getColor(getContext(), R.color.view_gardenWeather_maxTemperature);
        maxTempSet.setColor(maxTemperatureColor);
        maxTempSet.setDrawCircles(false);

        return maxTempSet;
    }

    @NotNull
    private LineDataSet generateMinTempLineChart(List<Forecast> forecastsList) {

        ArrayList<Entry> minTempLineChartEntries = new ArrayList<>();

        for (int i = 0; i < forecastsList.size(); i++) {
            minTempLineChartEntries.add(new BarEntry(i, forecastsList.get(i).getMinAirTemperatureInCelsius().floatValue()));
        }

        // get min values for temperature
        minTemp = Collections.min(minTempLineChartEntries, Comparator.comparing(s -> s.getY())).getY();

        LineDataSet minTempSet = new LineDataSet(minTempLineChartEntries, "");
        YAxis leftAxis = combinedTemperatureRainfallChart.getAxisLeft();
        minTempSet.setAxisDependency(leftAxis.getAxisDependency());
        minTempSet.setMode(LineDataSet.Mode.CUBIC_BEZIER); // for smooth line chart
        minTempSet.setLineWidth(2);
        int minTemperatureColor = ContextCompat.getColor(getContext(), R.color.view_gardenWeather_minTemperature);
        minTempSet.setColor(minTemperatureColor);
        minTempSet.setDrawCircles(false);
        return minTempSet;
    }

    @NotNull
    private BarDataSet generatePrecipitationBarChart(List<Forecast> forecastsList) {
        ArrayList<BarEntry> barRainFallChartEntries = new ArrayList<>();
        for (int i = 0; i < forecastsList.size(); i++) {
            barRainFallChartEntries.add(new BarEntry(i + 0.2f, forecastsList.get(i).getPrecipitationAmountInMillimeter().floatValue()));
        }

        // get max values for Precipitation
        maxPrecipitation = Collections.max(forecastsList, Comparator.comparing(s -> s.getPrecipitationAmountInMillimeter())).getPrecipitationAmountInMillimeter();
        // get min values for Precipitation
        minPrecipitation = Collections.min(forecastsList, Comparator.comparing(s -> s.getPrecipitationAmountInMillimeter())).getPrecipitationAmountInMillimeter();

        BarDataSet precipitationDataSet = new BarDataSet(barRainFallChartEntries, "");
        YAxis rightAxis = combinedTemperatureRainfallChart.getAxisRight();
        precipitationDataSet.setAxisDependency(rightAxis.getAxisDependency());
        precipitationDataSet.setDrawValues(false);

        int rainFallColor = ContextCompat.getColor(getContext(), R.color.view_gardenWeather_rainfall);
        precipitationDataSet.setColor(rainFallColor);

        return precipitationDataSet;
    }

    private float setMinTemperature() {
        if (minTemp < TEMPERATURE_0_CELSIUS) {
            return minTemp - PADDING_1;
        } else {
            return TEMPERATURE_0_CELSIUS;
        }
    }

    private void configureCombinedChartAppearance() {

        combinedTemperatureRainfallChart.getAxisLeft().setStartAtZero(false);
        combinedTemperatureRainfallChart.getAxisRight().setStartAtZero(false);

        // padding
        combinedTemperatureRainfallChart.getAxisRight().setSpaceMax(0.5f);
        combinedTemperatureRainfallChart.getAxisLeft().setSpaceMax(0.5f);

        combinedTemperatureRainfallChart.getDescription().setEnabled(false);
        combinedTemperatureRainfallChart.setDrawValueAboveBar(false);

        // Hide Background grid
        combinedTemperatureRainfallChart.getXAxis().setDrawGridLines(false);
        combinedTemperatureRainfallChart.getAxisLeft().setDrawGridLines(false);
        combinedTemperatureRainfallChart.getAxisRight().setDrawGridLines(false);
        // hide legends
        combinedTemperatureRainfallChart.getLegend().setEnabled(false);

        XAxis xAxis = combinedTemperatureRainfallChart.getXAxis();
        xAxis.setValueFormatter(new ValueFormatter() {
            @Override
            public String getFormattedValue(float value) {
                return DAYS[(int) value];
            }
        });
        YAxis axisLeft = combinedTemperatureRainfallChart.getAxisLeft();
        axisLeft.setLabelCount(6);
        axisLeft.setValueFormatter(new ValueFormatter() {
            @Override
            public String getFormattedValue(float value) {
                return String.format((int) value + " °C");
            }
        });
       // axisLeft.setTextSize(getResources().getDimension(R.dimen.text_size_chart));

        YAxis axisRight = combinedTemperatureRainfallChart.getAxisRight();
        axisRight.setLabelCount(6);
        axisRight.setValueFormatter(new ValueFormatter() {
            @Override
            public String getFormattedValue(float value) {
                return String.format((int) value + " l/m\u00B2");
            }
        });

    }

    @Override
    public void onResume() {
        super.onResume();
        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, (Activity) getContext(), true, null);
        }
    }
}