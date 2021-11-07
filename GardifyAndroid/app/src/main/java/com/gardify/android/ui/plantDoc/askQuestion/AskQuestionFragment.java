package com.gardify.android.ui.plantDoc.askQuestion;

import android.Manifest;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.GridView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.Toast;

import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;

import com.android.volley.DefaultRetryPolicy;
import com.android.volley.Request;
import com.android.volley.VolleyLog;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.R;
import com.gardify.android.generic.GenericDialog;
import com.gardify.android.generic.SaveImageToGallery;
import com.gardify.android.ui.plantDoc.ImageAdapter;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.VolleyMultipartRequest;

import org.apache.http.HttpEntity;
import org.apache.http.entity.ContentType;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntityBuilder;
import org.apache.http.entity.mime.content.StringBody;
import org.json.JSONObject;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import static android.app.Activity.RESULT_OK;
import static com.gardify.android.utils.ImageUtils.convertUriToBitmap;
import static com.gardify.android.utils.ImageUtils.getImageBytes;
import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class AskQuestionFragment extends Fragment {

    public static final int EXTERNAL_STORAGE_WRITE_REQUEST_CODE = 1011;
    private static final int IMAGE_CAMERA = 1000;
    private static final int IMAGE_ALBUM = 1001;
    private static int count = 0;

    private ProgressBar progressBar;
    private CardView cardViewCamera;
    private CardView cardViewAlbum;
    private CheckBox checkBox;
    private Button buttonSendQuestion;
    private EditText editTextTheme;
    private EditText editTextQuestion;
    private LinearLayout linearLayoutPlantDocAskQuestion;
    private Uri imageUri;

    private GridView gridView;
    private ArrayList<Bitmap> arrayList;
    private ImageAdapter imageAdapter;
    private Bitmap bitmap, scanResultBitmap;
    private byte[] image;


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        setupToolbar(getActivity(), "PFLANZEN-DOC", R.drawable.gardify_app_icon_pflanzendoc, R.color.toolbar_plantDoc_setupToolbar,true);

        View root = inflater.inflate(R.layout.fragment_plant_doc_ask_question, container, false);

        linearLayoutPlantDocAskQuestion = root.findViewById(R.id.linearLayout_plant_doc_ask_question);
        progressBar = root.findViewById(R.id.progressBar_plant_doc_ask_question);

        View rootInToLinearLayout = getLayoutInflater().inflate(R.layout.view_plant_doc_ask_question, linearLayoutPlantDocAskQuestion, false);

        Init(rootInToLinearLayout);

        arrayList = new ArrayList<>();

        Bundle resultBundle = this.getArguments();
        if(resultBundle != null) {
            image = resultBundle.getByteArray("IMAGE");
            scanResultBitmap = BitmapFactory.decodeByteArray(image, 0, image.length);
            arrayList.add(scanResultBitmap);
            imageAdapter = new ImageAdapter(getContext(), arrayList);
            gridView.setAdapter(imageAdapter);
        }

        buttonSendQuestion.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                try {
                    if (validateUserInput()) {

                        String textTheme = editTextTheme.getText().toString();
                        String textQuestion = editTextQuestion.getText().toString();

                        HashMap<String, String> params = new HashMap<>();
                        params.put("Description", "");
                        params.put("Isownfoto", String.valueOf(checkBox.isChecked()));
                        params.put("QuestionText", textQuestion);
                        params.put("Thema", textTheme);

                        JSONObject jsonObject = new JSONObject(params);

                        String apiUrlPost = APP_URL.PLANT_DOC_ROUTE + "newEntry";

                        RequestQueueSingleton.getInstance(getContext()).stringRequest(apiUrlPost, Request.Method.POST, this::send, null, jsonObject);
                        progressBar.setVisibility(View.VISIBLE);
                    }

                } catch (Exception e) {
                    Toast.makeText(getContext(), e.toString(), Toast.LENGTH_LONG).show();

                }
            }

            private void send(String jsonObject) {
                if (jsonObject != null) {
                    sendImage(jsonObject);

                }
            }
        });

        cardViewCamera.setOnClickListener(v -> {
            Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
            startActivityForResult(intent, IMAGE_CAMERA);
        });

        cardViewAlbum.setOnClickListener(v -> {
            Intent intent = new Intent(Intent.ACTION_PICK, MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
            startActivityForResult(intent, IMAGE_ALBUM);
        });

        linearLayoutPlantDocAskQuestion.addView(rootInToLinearLayout);

        return root;
    }

    private void Init(View view) {
        cardViewCamera = view.findViewById(R.id.cardView_view_plant_doc_ask_question_camera);
        cardViewAlbum = view.findViewById(R.id.cardView_view_plant_doc_ask_question_album);
        checkBox = view.findViewById(R.id.checkBox_plant_doc_ask_question);
        buttonSendQuestion = view.findViewById(R.id.button_plant_doc_ask_question);
        editTextTheme = view.findViewById(R.id.editTextTextPersonName_plant_doc_ask_theme);
        editTextQuestion = view.findViewById(R.id.editTextTextPersonName_plant_doc_ask_question);
        gridView = view.findViewById(R.id.gridView_plant_doc_ask_question);
    }

    private void sendImage(String id) {

        //String url = "http://httpbin.org/post";
        String apiURL = APP_URL.PLANT_DOC_ROUTE + "upload";

        //params
        HashMap<String, String> params = new HashMap<>();
        params.put("id", id);

        HttpEntity httpEntity;
        MultipartEntityBuilder builder = MultipartEntityBuilder.create();
        builder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);

        // Add binary body
        ContentType contentType = ContentType.create("image/jpeg");
        String fileName = "question_image.jpg";
        int i = 0;
        for (Bitmap bitmap : arrayList) {
            builder.addBinaryBody("img", getImageBytes(bitmap), contentType, fileName);
            params.put("imageTitle" + i, fileName);
            i++;
        }

        // adding params
        for (String key : params.keySet()) {
            builder.addPart(key, new StringBody(params.get(key), ContentType.MULTIPART_FORM_DATA.withCharset("UTF-8")));
        }

        httpEntity = builder.build();

        VolleyMultipartRequest myRequest = new VolleyMultipartRequest(Request.Method.POST, apiURL, response -> {
            progressBar.setVisibility(View.GONE);
            displayAlertDialog(getContext(),getResources().getString(R.string.plantDoc_questionAsked));
            navigateToFragment(R.id.nav_controller_plant_doc, (Activity) getContext(), true, null);
        }, error -> {
            Toast.makeText(getContext(), error.toString(), Toast.LENGTH_SHORT).show();
            progressBar.setVisibility(View.GONE);
        }) {
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

    public Boolean validateUserInput() {

        boolean validFlag = true;

        if (editTextTheme.getText().toString().trim().length() == 0) {
            validFlag = false;
            editTextTheme.setError(getContext().getResources().getString(R.string.all_required));
        } else if (editTextQuestion.getText().toString().trim().length() == 0) {
            validFlag = false;
            editTextQuestion.setError(getContext().getResources().getString(R.string.all_required));
        } else if (arrayList.size() == 0) {
            validFlag = false;
            displayAlertDialog(getContext(), getResources().getString(R.string.plantDoc_noImage));
        }
        return validFlag;
    }
}