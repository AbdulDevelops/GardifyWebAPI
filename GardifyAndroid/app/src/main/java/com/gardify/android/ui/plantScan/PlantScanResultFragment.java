package com.gardify.android.ui.plantScan;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.fragment.app.Fragment;

import com.android.volley.Request;
import com.android.volley.RetryPolicy;
import com.android.volley.VolleyError;
import com.android.volley.VolleyLog;
import com.gardify.android.R;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.data.plantScan.DoubleResult;
import com.gardify.android.data.plantScan.Family;
import com.gardify.android.data.plantScan.InDb;
import com.gardify.android.data.plantScan.PlantScan;
import com.gardify.android.data.plantScan.Result;
import com.gardify.android.data.settings.UserInfo;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.google.gson.Gson;

import org.apache.http.HttpEntity;
import org.apache.http.entity.ContentType;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntityBuilder;
import org.jetbrains.annotations.NotNull;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import static androidx.constraintlayout.motion.utils.Oscillator.TAG;
import static com.gardify.android.ui.saveToGarden.SaveToGardenFragment.PLANT_ID_ARG;
import static com.gardify.android.utils.ImageUtils.getImageBytes;
import static com.gardify.android.utils.StringUtils.formatHtmlKTags;
import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class PlantScanResultFragment extends Fragment {

    byte[] byteArrayImage;
    String apiUrl = APP_URL.PLANT_SCAN_API;
    String suggestionApiUrl = APP_URL.PLANT_SUGGESTION_API;
    TextView textViewDatabase, textViewPlantNet;
    LinearLayout linearLayoutPlanScanDb;
    LinearLayout linearLayoutPlanScanPlantNet;
    ProgressBar progressBar;
    String imageLink;
    String resultResponse;

    String imageTitle = "";
    String Name = "";
    String NameLatin = "";
    String searchResult = "";
    Boolean isAuthor = false;


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            byteArrayImage = getArguments().getByteArray("IMAGE");
            getArguments().clear();
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_plant_scanner, container, false);

        setupToolbar(getActivity(), "PFLANZEN SCAN", R.drawable.gardify_app_icon_pflanzen_erkennen, R.color.toolbar_plantScan_setupToolbar, true);

        textViewDatabase = view.findViewById(R.id.textView_plantScan_db);
        textViewPlantNet = view.findViewById(R.id.textView_plantScan_planetNet);
        linearLayoutPlanScanDb = view.findViewById(R.id.linearLayout_plant_scan_db);
        linearLayoutPlanScanPlantNet = view.findViewById(R.id.linearLayout_plant_plantNet);
        progressBar = view.findViewById(R.id.progressBar_plant_scanner);

        HttpEntity httpEntity;

        //String url = "http://httpbin.org/post";
        MultipartEntityBuilder builder = MultipartEntityBuilder.create();
        builder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);

        // Add binary body
        if (byteArrayImage != null) {
            ContentType contentType = ContentType.create("image/jpeg");
            String fileName = "p.jpg";
            builder.addBinaryBody("img", byteArrayImage, contentType, fileName);
            httpEntity = builder.build();

            VolleyMultipartRequest myRequest = new VolleyMultipartRequest(Request.Method.POST, apiUrl, response -> {
                //String resultResponse = new String(response.data);
                resultResponse = new String(response.data);
                PlantScan object = new Gson().fromJson(resultResponse, PlantScan.class);
                onSuccess(object);

            }, error -> Toast.makeText(getContext(), error.toString(), Toast.LENGTH_SHORT).show()) {
                @Override
                public String getBodyContentType() {
                    return httpEntity.getContentType().getValue();
                }

                @Override
                public byte[] getBody() {
                    ByteArrayOutputStream bos = new ByteArrayOutputStream();
                    try {
                        httpEntity.writeTo(bos);
                    } catch (IOException e) {
                        VolleyLog.e("IOException writing to ByteArrayOutputStream");
                    }
                    return bos.toByteArray();
                }
            };
            myRequest.setRetryPolicy(new RetryPolicy() {
                @Override
                public int getCurrentTimeout() {
                    return 50000;
                }

                @Override
                public int getCurrentRetryCount() {
                    return 5;
                }

                @Override
                public void retry(VolleyError error) {

                }
            });
            RequestQueueSingleton.getInstance(getContext()).addToRequestQueue(myRequest);
        }

        return view;

    }

    public void onSuccess(PlantScan plantScan) {
        Activity activity = getActivity();
        if (activity != null) {
            progressBar.setVisibility(View.GONE);

            List<Result> plantNetResult = plantScan.getPnResults().getResults();
            List<InDb> databaseResult = plantScan.getPnResults().getInDb();
            List<DoubleResult> doubleResults = plantScan.getPnResults().getDoubleResult();

            //plantNetResult = removePlantNetDuplicates(plantNetResult, doubleResults);
            if (databaseResult.size() != 0 && plantNetResult.size() != 0) {
                //getting all ScientificNameWithoutAuthor for all plantNetResult
                for (int i = 0; i < plantNetResult.size(); i++) {
                    searchResult += plantNetResult.get(i).getSpecies().getScientificNameWithoutAuthor() + "\n";
                }
                Log.d(TAG, "onSuccess: " + searchResult);


                if (databaseResult.size() != 0) {
                    for (InDb db : databaseResult) {

                        View rootInToLinearLayoutResult = getLayoutInflater().inflate(R.layout.view_plant_scan_result, linearLayoutPlanScanDb, false);

                        textViewDatabase.setVisibility(View.VISIBLE);

                        ImageView imageViewResult = rootInToLinearLayoutResult.findViewById(R.id.imageView_view_plant_scan_result);
                        TextView textViewLatinName = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_latin);
                        textViewLatinName.setText(formatHtmlKTags(db.getNameLatin()));

                        TextView textViewFamilyNameLabel = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_plant_family_name_label);
                        textViewFamilyNameLabel.setText(R.string.plantSearch_plantFamily + ": ");

                        TextView textViewFamilyName = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_plant_family_name);
                        String dbFamily = (String) db.getFamily();
                        if (dbFamily != null) {
                            textViewFamilyName.setVisibility(View.VISIBLE);
                            textViewFamilyName.setText(db.getFamily().toString());
                        } else textViewFamilyName.setVisibility(View.GONE);

                        TextView textViewGermanName = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_plant_german_name);
                        if (db.getNameGerman() != null) {
                            textViewGermanName.setVisibility(View.VISIBLE);
                            textViewGermanName.setText(db.getNameGerman());
                        } else {
                            textViewGermanName.setVisibility(View.GONE);
                        }

                        TextView textViewSynonym = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_synonym);
                        TextView textViewSynonymLabel = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_synonym_label);
                        if (db.getSynonym() != null) {
                            textViewSynonymLabel.setVisibility(View.VISIBLE);
                            textViewSynonymLabel.setText(getResources().getString(R.string.plantScan_result_synonym) + ": ");
                            textViewSynonym.setVisibility(View.VISIBLE);
                            textViewSynonym.setText(formatHtmlKTags(db.getSynonym()));
                        } else {
                            textViewSynonymLabel.setVisibility(View.GONE);
                            textViewSynonym.setVisibility(View.GONE);
                        }

                        TextView textViewInformation = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_information);
                        if (db.getDescription() != null) {
                            textViewInformation.setVisibility(View.VISIBLE);
                            textViewInformation.setMaxLines(5);
                            textViewInformation.setText(formatHtmlKTags(db.getDescription()));
                        } else {
                            textViewInformation.setVisibility(View.GONE);
                        }

                        Button btnReadMore = rootInToLinearLayoutResult.findViewById(R.id.button_view_plant_scan_result_more_information);
                        btnReadMore.setOnClickListener(v -> {
                            Bundle bundle = new Bundle();
                            bundle.putString(PLANT_ID_ARG, String.valueOf(db.getId()));
                            navigateToFragment(R.id.nav_controller_plant_detail, getActivity(), false, bundle);

                        });
                        btnReadMore.setVisibility(View.VISIBLE);

                        Button btnSaveInToMyGarden = rootInToLinearLayoutResult.findViewById(R.id.button_view_plant_scan_result_save_into_my_garden);
                        btnSaveInToMyGarden.setOnClickListener(v -> {
                            Bundle bundle = new Bundle();
                            bundle.putString(PLANT_ID_ARG, String.valueOf(db.getId()));
                            navigateToFragment(R.id.nav_controller_save_to_garden, getActivity(), false, bundle);

                        });
                        btnSaveInToMyGarden.setVisibility(View.VISIBLE);

                        imageLink = APP_URL.BASE_ROUTE_INTERN + db.getImages().get(0).getSrcAttr();
                        loadImageUsingGlide(getContext(), imageLink, imageViewResult);
                        linearLayoutPlanScanDb.addView(rootInToLinearLayoutResult);

                    }
                }

                for (Result results : plantNetResult) {
                    textViewPlantNet.setVisibility(View.VISIBLE);

                    View rootInToLinearLayoutResult = getLayoutInflater().inflate(R.layout.view_plant_scan_result, linearLayoutPlanScanPlantNet, false);

                    ImageView imageViewResult = rootInToLinearLayoutResult.findViewById(R.id.imageView_view_plant_scan_result);

                    TextView textViewLatinName = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_latin);
                    textViewLatinName.setText(results.getSpecies().getScientificNameWithoutAuthor());

                    TextView textViewFamilyNameLabel = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_plant_family_name_label);
                    textViewFamilyNameLabel.setText(getResources().getString(R.string.plantSearch_plantFamily) + ": ");

                    TextView textViewFamilyName = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_plant_family_name);
                    Family pnFamily = results.getSpecies().getFamily();
                    if (pnFamily != null) {
                        textViewFamilyName.setVisibility(View.VISIBLE);
                        textViewFamilyName.setText(pnFamily.getScientificNameWithoutAuthor());
                    } else textViewFamilyName.setVisibility(View.GONE);

                    List<String> commonNames = results.getSpecies().getCommonNames();

                    TextView textViewGermanName = rootInToLinearLayoutResult.findViewById(R.id.textView_view_plant_scan_result_plant_german_name);
                    if (commonNames != null) {
                        String listString = "";
                        for (int i = 0; i < commonNames.size(); i++) {
                            listString += commonNames.get(i);
                            if (i < (commonNames.size() - 1))
                                listString += ", ";
                        }
                        textViewGermanName.setVisibility(View.VISIBLE);
                        textViewGermanName.setText(listString);
                    } else {
                        textViewGermanName.setVisibility(View.GONE);
                    }

                    Button btnOpenInGoogle = rootInToLinearLayoutResult.findViewById(R.id.button_view_plant_scan_result_open_in_google);
                    btnOpenInGoogle.setOnClickListener(v -> {
                        String[] split = (results.getSpecies().getScientificNameWithoutAuthor()).split("\\s+");
                        String link;
                        if (split.length > 1) {
                            link = "https://www.google.com/search?q=" + split[0] + "+" + split[1];
                        } else link = "https://www.google.com/search?q=" + split[0];
                        Uri uri = Uri.parse(link);
                        Intent intent = new Intent(Intent.ACTION_VIEW, uri);
                        getActivity().startActivity(intent);
                    });
                    btnOpenInGoogle.setVisibility(View.VISIBLE);

                    Button btnSuggestPlant = rootInToLinearLayoutResult.findViewById(R.id.button_view_plant_scan_result_suggest_plant);
                    btnSuggestPlant.setOnClickListener(v -> {
                        suggestionDialog(results, imageViewResult);

                    });
                    btnSuggestPlant.setVisibility(View.VISIBLE);

                    loadImageUsingGlide(getContext(), results.getImages().get(0).getLink(), imageViewResult);

                    linearLayoutPlanScanPlantNet.addView(rootInToLinearLayoutResult);

                }
            } else {
                displayAlertDialog(getContext(), getResources().getString(R.string.plantScan_scanNotSuccessful));
                Bundle imageBundle = new Bundle();
                imageBundle.putBoolean("success", false);
                imageBundle.putByteArray("IMAGE", byteArrayImage);
                navigateToFragment(R.id.nav_controller_plant_scan, getActivity(), true, imageBundle);
            }
        }
    }

    @NotNull
    private List<Result> removePlantNetDuplicates(List<Result> plantNetResult, List<DoubleResult> doubleResults) {
        return plantNetResult.stream()
                .filter(x -> doubleResults.stream()
                        .anyMatch(one -> !one.getSpecies().getScientificNameWithoutAuthor().contains(x.getSpecies().getScientificNameWithoutAuthor())))
                .collect(Collectors.toList());
    }

    private void suggestionDialog(Result result, ImageView imageViewResult) {
        AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(getContext());
        LayoutInflater inflater = this.getLayoutInflater();
        View dialogView = inflater.inflate(R.layout.popup_dialog_default_two_buttons, null);

        Button positiveButton = dialogView.findViewById(R.id.button_popup_dialog_green);
        Button negativeButton = dialogView.findViewById(R.id.button_popup_dialog_red);

        TextView headerTextView = dialogView.findViewById(R.id.button_popup_dialog_header);
        TextView descTextView = dialogView.findViewById(R.id.button_popup_dialog_description);

        headerTextView.setText(getResources().getString(R.string.plantScan_suggestPlant));
        descTextView.setText(getResources().getString(R.string.plantScan_creator_of_picture));
        positiveButton.setText(getResources().getString(R.string.plantScan_yes));
        negativeButton.setText(getResources().getString(R.string.plantScan_no));

        // show dialog
        dialogBuilder.setView(dialogView);
        AlertDialog alertDialog = dialogBuilder.create();
        alertDialog.setCancelable(true);
        alertDialog.show();

        // button click listeners
        negativeButton.setOnClickListener(view -> alertDialog.dismiss());

        positiveButton.setOnClickListener(view -> {
            alertDialog.dismiss();
            doesSuggestion(result, imageViewResult);
            //suggestionCompleteDialog();
        });
    }

    private void doesSuggestion(Result result, ImageView imageViewResult) {
        HttpEntity httpEntity;

        //String url = "http://httpbin.org/post";
        MultipartEntityBuilder builder = MultipartEntityBuilder.create();
        builder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);

        BitmapDrawable drawable1 = (BitmapDrawable) imageViewResult.getDrawable();
        Bitmap bitmap = drawable1.getBitmap();
        byteArrayImage = getImageBytes(bitmap);

        imageTitle = result.getSpecies().getScientificNameWithoutAuthor();
        imageLink = result.getImages().get(0).getLink();
        Name = result.getSpecies().getScientificNameWithoutAuthor();
        NameLatin = result.getSpecies().getScientificNameWithoutAuthor();
        isAuthor = true;
        Log.d(TAG, "onParamsCheck: " + Name + " " + imageLink);

        // Add binary body
        if (byteArrayImage != null) {
            ContentType contentType = ContentType.create("image/jpeg");
            String fileName = NameLatin + ".jpg";
            builder.addBinaryBody("imageFile", byteArrayImage, contentType, fileName);

            builder.addTextBody("imageTitle", fileName, ContentType.DEFAULT_TEXT);
            builder.addTextBody("Name", Name, ContentType.DEFAULT_TEXT);
            builder.addTextBody("NameLatin", NameLatin, ContentType.DEFAULT_TEXT);
            builder.addTextBody("searchResult", searchResult, ContentType.DEFAULT_TEXT);
            isAuthor = true;
            builder.addTextBody("isAuthor", isAuthor.toString(), ContentType.DEFAULT_TEXT);

            httpEntity = builder.build();

            VolleyMultipartRequest myRequest = new VolleyMultipartRequest(Request.Method.POST, suggestionApiUrl, response -> {
                String resultResponse = new String(response.data);
                suggestionCompleteDialog(resultResponse);
            }, error -> Toast.makeText(getContext(), error.toString(), Toast.LENGTH_SHORT).show()) {
                @Override
                public String getBodyContentType() {
                    return httpEntity.getContentType().getValue();
                }

                @Override
                public Map<String, String> getHeaders() {
                    ApplicationUser user = PreferencesUtility.getUser(getContext());
                    Map<String, String> headers = new HashMap<>();
                    headers.put("Authorization", "Bearer " + user.getToken());
                    return headers;
                }

                @Override
                public byte[] getBody() {
                    ByteArrayOutputStream bos = new ByteArrayOutputStream();
                    try {
                        httpEntity.writeTo(bos);
                    } catch (IOException e) {
                        VolleyLog.e("IOException writing to ByteArrayOutputStream");
                    }
                    return bos.toByteArray();
                }
            };
            myRequest.setRetryPolicy(new RetryPolicy() {
                @Override
                public int getCurrentTimeout() {
                    return 50000;
                }

                @Override
                public int getCurrentRetryCount() {
                    return 5;
                }

                @Override
                public void retry(VolleyError error) {

                }
            });
            RequestQueueSingleton.getInstance(getContext()).addToRequestQueue(myRequest);
        }
    }

    public void onSuggestionSuccess(String resultResponse) {
        Log.d(TAG, "onSuggestionSuccess: " + resultResponse);
    }

    private void suggestionCompleteDialog(String resultResponse) {
        ApplicationUser user = PreferencesUtility.getUser(getContext());
        UserInfo model = new UserInfo();

        if (resultResponse == null)
            Toast.makeText(getContext(), "Irgendwas stimmt nicht!", Toast.LENGTH_SHORT).show();

        AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(getContext());
        LayoutInflater inflater = this.getLayoutInflater();
        View dialogView = inflater.inflate(R.layout.popup_dialog_default, null);

        Button positiveButton = dialogView.findViewById(R.id.button_popup_dialog_close);

        TextView headerTextView = dialogView.findViewById(R.id.text_view_default_popup_dialog);

        headerTextView.setText(getResources().getString(R.string.all_hello) + " " + user + getResources().getString(R.string.all_suggestion_message));
        positiveButton.setText(getResources().getString(R.string.all_close));

        // show dialog
        dialogBuilder.setView(dialogView);
        AlertDialog alertDialog = dialogBuilder.create();
        alertDialog.setCancelable(true);
        alertDialog.show();

        positiveButton.setOnClickListener(view -> alertDialog.dismiss());
    }


}