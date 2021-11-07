package com.gardify.android.ui.suggestPlants;

import android.Manifest;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.ColorStateList;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.text.SpannableString;
import android.text.Spanned;
import android.text.method.LinkMovementMethod;
import android.text.style.ClickableSpan;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.GridView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;

import com.android.volley.DefaultRetryPolicy;
import com.android.volley.Request;
import com.android.volley.VolleyLog;
import com.gardify.android.R;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.generic.GenericDialog;
import com.gardify.android.generic.SaveImageToGallery;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.VolleyMultipartRequest;

import org.apache.http.HttpEntity;
import org.apache.http.entity.ContentType;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntityBuilder;
import org.apache.http.entity.mime.content.StringBody;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import static android.app.Activity.RESULT_OK;
import static com.gardify.android.ui.plantSearch.PlantSearchFragment.SEARCH_TEXT_ARG;
import static com.gardify.android.utils.ImageUtils.convertUriToBitmap;
import static com.gardify.android.utils.ImageUtils.getImageBytes;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class SuggestPlantsFragment extends Fragment {

    public static final int EXTERNAL_STORAGE_WRITE_REQUEST_CODE = 1011;
    private static final int IMAGE_CAMERA = 1000;
    private static final int IMAGE_ALBUM = 1001;
    private static final int PLANT_TAKE_COUNT = 10;

    private static final String TAG = "SuggestPlantsFragment";

    private static int count = 0;
    String plantNameFromCount;
    boolean isAuthor = false;

    ProgressBar progressBar;
    CardView cardViewCamera;
    CardView cardViewAlbum;
    Button buttonSendQuestion;
    EditText editTextPlantName;
    TextView textViewCopyright;

    Uri imageUri;

    GridView gridView;
    ArrayList<Bitmap> arrayList;
    ImageAdapter imageAdapter;
    Bitmap bitmap;


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        setupToolbar(getActivity(), "PFLANZEN ERGÄNZEN", R.drawable.gardify_app_icon_pflanze_ergaenzen, R.color.toolbar_suggestPlant_setupToolbar, true);

        View root = inflater.inflate(R.layout.fragment_suggest_plants, container, false);

        initializeViews(root);

        arrayList = new ArrayList<>();

        buttonSendQuestion.setOnClickListener(v -> {
            getTotalCountFromApi();
            progressBar.setVisibility(View.VISIBLE);
        });

        cardViewCamera.setOnClickListener(v -> {
            Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
            startActivityForResult(intent, IMAGE_CAMERA);
        });

        cardViewAlbum.setOnClickListener(v -> {
            Intent intent = new Intent(Intent.ACTION_PICK, MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
            startActivityForResult(intent, IMAGE_ALBUM);
        });

        return root;
    }

    private void initializeViews(View root) {
        cardViewCamera = root.findViewById(R.id.cardView_suggest_plant_camera);
        cardViewAlbum = root.findViewById(R.id.cardView_suggest_plant_album);
        buttonSendQuestion = root.findViewById(R.id.button_suggest_plant);
        editTextPlantName = root.findViewById(R.id.editTextTextPersonName_suggest_plant_name);
        gridView = root.findViewById(R.id.gridView_suggest_plant);
        progressBar = root.findViewById(R.id.progressbar_suggestPlants);
        textViewCopyright = root.findViewById(R.id.textView_suggest_plant_copyright);

        textViewClickable();
    }

    private void sendPostRequest() {
        String scanEmailUrl = APP_URL.ACCOUNT_API + "AddPlant";
        //String scanEmailUrl = "http://httpbin.org/post";

        String plantName = editTextPlantName.getText().toString();

        HashMap<String, String> params = new HashMap<>();

        params.put("name", plantName);
        params.put("ignoreMatches", "false");
        params.put("known", "true");
        params.put("isAuthor", String.valueOf(isAuthor));
        params.put("searchResult", "");

        HttpEntity httpEntity;
        MultipartEntityBuilder builder = MultipartEntityBuilder.create();
        builder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);

        // Add binary body
        ContentType contentType = ContentType.create("image/jpeg");
        String fileName = "image.jpg";
        int i = 0;
        for (Bitmap bitmap : arrayList) {
            builder.addBinaryBody("imageFile" + i, getImageBytes(bitmap), contentType, fileName);
            params.put("imageTitle" + i, fileName);
            i++;
        }

        // adding params
        for (String key : params.keySet()) {
            builder.addPart(key, new StringBody(params.get(key), ContentType.MULTIPART_FORM_DATA.withCharset("UTF-8")));
        }

        httpEntity = builder.build();

        VolleyMultipartRequest myRequest = new VolleyMultipartRequest(Request.Method.POST, scanEmailUrl, response -> {
            showSuccessDialog();
            progressBar.setVisibility(View.GONE);

        }, error ->
                Toast.makeText(getContext(), error.toString(), Toast.LENGTH_SHORT).show()) {
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

        myRequest.setRetryPolicy(new DefaultRetryPolicy(10000, 2,
                DefaultRetryPolicy.DEFAULT_BACKOFF_MULT));

        RequestQueueSingleton.getInstance(getContext()).addToRequestQueue(myRequest);


    }

    private void getTotalCountFromApi() {
        plantNameFromCount = editTextPlantName.getText().toString();

        String searchCountUrl = APP_URL.PLANT_SEARCH_TOTAL_COUNT + "/?searchText=" + plantNameFromCount;
        RequestQueueSingleton.getInstance(getContext()).stringRequest(searchCountUrl, Request.Method.GET, this::onSuccessCount, null, null);
    }

    private void onSuccessCount(String plantCount) {
        int plantResultCount = Integer.parseInt(plantCount);
        if (plantResultCount == 0) {
            askQuestionDialog();
        } else {
            showAlreadyInDbDialog();
        }

        /*if (getArguments() != null) {
            showPlantForPassedQuery(plantResultCount);
        }*/
    }


    private void showSuccessDialog() {
        ApplicationUser user = PreferencesUtility.getUser(getContext());

        new GenericDialog.Builder(getContext())
                .setTitle(getResources().getString(R.string.all_hello) + " " + user.getName())
                .setTitleAppearance(R.color.text_all_gunmetal, R.dimen.textSize_body_medium)
                .setMessage(getResources().getString(R.string.all_suggestion_message))
                .setMessageAppearance(R.color.text_all_riverBed, R.dimen.textSize_body_small)
                .addNewButton(R.style.TransparentButtonStyle,
                        getResources().getString(R.string.all_close), R.dimen.textSize_body_medium, view -> {
                        })
                .setButtonOrientation(LinearLayout.HORIZONTAL)
                .setCancelable(true)
                .generate();
    }

    private void showAlreadyInDbDialog() {
        ApplicationUser user = PreferencesUtility.getUser(getContext());

        new GenericDialog.Builder(getContext())
                .setTitle(getResources().getString(R.string.all_hello) + " " + user.getName())
                .setTitleAppearance(R.color.text_all_gunmetal, R.dimen.textSize_body_medium)
                .setMessage(getResources().getString(R.string.suggestPlant_already_exists))
                .setMessageAppearance(R.color.text_all_riverBed, R.dimen.textSize_body_small)
                .addNewButton(R.style.TransparentButtonStyle,
                        getResources().getString(R.string.all_close), R.dimen.textSize_body_medium, view -> {
                            //navigate to search fragment
                            Bundle args = new Bundle();
                            args.putString(SEARCH_TEXT_ARG, plantNameFromCount);
                            navigateToFragment(R.id.nav_controller_plant_search, getActivity(), true, args);

                        })
                .setButtonOrientation(LinearLayout.HORIZONTAL)
                .setCancelable(true)
                .generate();
    }

    private void askQuestionDialog() {
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
        negativeButton.setOnClickListener(view -> {
            alertDialog.dismiss();
            sendPostRequest();
        });

        positiveButton.setOnClickListener(view -> {
            alertDialog.dismiss();
            isAuthor = true;
            sendPostRequest();
        });
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK && requestCode == IMAGE_CAMERA) {
            bitmap = (Bitmap) data.getExtras().get("data");
            arrayList.add(bitmap);
            showSaveToGalleryDialog();
        } else if (resultCode == RESULT_OK && requestCode == IMAGE_ALBUM) {
            imageUri = data.getData();
            bitmap = convertUriToBitmap(getContext(), imageUri);
            arrayList.add(bitmap);
        }

        imageAdapter = new ImageAdapter(getContext(), arrayList);
        gridView.setAdapter(imageAdapter);
        Resources resources = getContext().getResources();
        buttonSendQuestion.setClickable(false);
        if (arrayList.size() == 0) {
            buttonSendQuestion.setBackgroundTintList(ColorStateList.valueOf(resources.getColor(R.color.button_all_sendClosePlusMinus)));
            buttonSendQuestion.setClickable(false);
        } else {
            buttonSendQuestion.setBackgroundTintList(ColorStateList.valueOf(resources.getColor(R.color.button_all_confirm)));
            buttonSendQuestion.setClickable(true);
        }
    }

    private void showSaveToGalleryDialog() {
        new GenericDialog.Builder(getContext())
                .setBitmapImage(bitmap)
                .setMessageAppearance(R.color.text_all_riverBed, R.dimen.textSize_body_small)
                .addNewButton(R.style.PrimaryButtonStyle,
                        getResources().getString(R.string.all_photo) + " " + getResources().getString(R.string.all_save), R.dimen.textSize_body_medium, view -> {
                            requestForWritePermission();
                        })
                .addNewButton(R.style.SecondaryButtonStyle,
                        getResources().getString(R.string.all_scan), R.dimen.textSize_body_medium, view -> {
                        })
                .setButtonOrientation(LinearLayout.VERTICAL)
                .setCancelable(true)
                .generate();
    }

    private void requestForWritePermission() {
        if (ContextCompat.checkSelfPermission(getActivity(), Manifest.permission.WRITE_EXTERNAL_STORAGE) ==
                PackageManager.PERMISSION_GRANTED) {
            SaveImageToGallery saveImageToGallery = new SaveImageToGallery(getContext());
            saveImageToGallery.saveImage(bitmap);
        } else {
            requestPermissions(new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE},
                    EXTERNAL_STORAGE_WRITE_REQUEST_CODE);
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           String permissions[], int[] grantResults) {
        switch (requestCode) {
            case EXTERNAL_STORAGE_WRITE_REQUEST_CODE: {
                if (isPermissionGranted(grantResults)) {
                    SaveImageToGallery saveImageToGallery = new SaveImageToGallery(getContext());
                    saveImageToGallery.saveImage(bitmap);
                } else {
                    Toast.makeText(getActivity(), "Berechtigung verweigert!", Toast.LENGTH_SHORT).show();
                }
                return;
            }
        }
    }

    private boolean isPermissionGranted(int[] grantResults) {
        return grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED;
    }

    private void textViewClickable() {
        String copyrightLink = "https://gardify.de/info";
        SpannableString spannableString = new SpannableString(getResources().getText(R.string.suggestPlant_copyright_hint));
        ClickableSpan clickableSpan = new ClickableSpan() {
            @Override
            public void onClick(@NonNull View widget) {
                Uri uri = Uri.parse(copyrightLink);
                Intent intent = new Intent(Intent.ACTION_VIEW, uri);
                getActivity().startActivity(intent);
            }
        };
        spannableString.setSpan(clickableSpan, 131, 143, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
        textViewCopyright.setMovementMethod(LinkMovementMethod.getInstance());
        textViewCopyright.setText(spannableString);
    }

    @Override
    public void onResume() {
        super.onResume();
        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, (Activity) getContext(), true, null);
        }
    }
}