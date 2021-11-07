package com.gardify.android.ui.settings.dialog;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.fragment.app.DialogFragment;

import com.android.volley.DefaultRetryPolicy;
import com.android.volley.Request;
import com.android.volley.VolleyLog;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.R;
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
import java.util.HashMap;
import java.util.Map;

import static android.app.Activity.RESULT_OK;
import static com.gardify.android.utils.ImageUtils.convertUriToBitmap;
import static com.gardify.android.utils.ImageUtils.getImageBytes;
import static com.gardify.android.utils.UiUtils.navigateToFragment;

public class UploadTypeDialogFragment extends DialogFragment {

    private static final String ACTION_ARGUMENT = "UPLOAD_TYPE";
    private static final int IMAGE_CAMERA = 1000;
    private static final int IMAGE_ALBUM = 1001;
    private byte[] byteArrayImage;
    private String uploadImage = APP_URL.ACCOUNT_API + "uploadProfilImg";
    ProgressBar progressBar;
    public UploadTypeDialogFragment() {
        // Empty constructor is required for DialogFragment
        // Make sure not to add arguments to the constructor
        // Use `newInstance` instead as shown below
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_settings_dialog_upload_type, container);
    }

    @Override
    public void onViewCreated(View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        // Get field from view
        TextView cameraTextView = view.findViewById(R.id.textView_settings_dialog_camera);
        TextView albumTextView = view.findViewById(R.id.textView_settings_dialog_album);
        progressBar = view.findViewById(R.id.progress_bar_dialog);

        cameraTextView.setOnClickListener(view1 -> {
            Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
            startActivityForResult(intent, IMAGE_CAMERA);
            progressBar.setVisibility(View.VISIBLE);
        });
        albumTextView.setOnClickListener(view1 -> {
            Intent intent = new Intent(Intent.ACTION_PICK, MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
            startActivityForResult(intent, IMAGE_ALBUM);
            progressBar.setVisibility(View.VISIBLE);
        });

    }


    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (resultCode == RESULT_OK && requestCode == IMAGE_CAMERA) {
            Bitmap bitmap = (Bitmap) data.getExtras().get("data");
            UploadBitmap(bitmap);
        }else if (resultCode == RESULT_OK && requestCode == IMAGE_ALBUM) {
            Uri imageUri = data.getData();
            Bitmap bitmap = convertUriToBitmap(getContext(), imageUri);
            UploadBitmap(bitmap);
        }
        progressBar.setVisibility(View.GONE);

    }

    private void UploadBitmap(Bitmap bitmap) {
        byteArrayImage = getImageBytes(bitmap);

        HashMap<String, String> params = new HashMap<>();
        params.put("imageTitle", "Image");


        HttpEntity httpEntity;
        MultipartEntityBuilder builder = MultipartEntityBuilder.create();
        builder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);

        ContentType contentType = ContentType.create("image/jpeg");
        String fileName = "profile_image.jpg";
        builder.addBinaryBody("imageFile", byteArrayImage, contentType, fileName);
        for (String key : params.keySet()) {
            builder.addPart(key, new StringBody(params.get(key), ContentType.MULTIPART_FORM_DATA.withCharset("UTF-8")));
        }

        httpEntity = builder.build();
        VolleyMultipartRequest myRequest = new VolleyMultipartRequest(Request.Method.POST, uploadImage, response -> {
            if (getContext() != null) {
                Toast.makeText(getContext(), "Bild wurde geändert", Toast.LENGTH_SHORT).show();
                refresh();
            }

        }, error -> {
            if (getContext() != null) {
                Toast.makeText(getContext(), error.toString(), Toast.LENGTH_SHORT).show();
                refresh();
            }
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

    private void refresh() {
        navigateToFragment(R.id.nav_controller_settings, (Activity) getContext(), true, null);
        dismiss();
    }
}