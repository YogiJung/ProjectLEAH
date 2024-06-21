package APICluster.TextToSpeechAPI;

import QueueManagers.SyncProcessQueueObject;
import com.google.gson.Gson;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.net.URI;
import java.nio.charset.StandardCharsets;

import static APICluster.APIClusterManager.*;

public class ReplicaStudioAPI {
    Gson gson = new Gson();
    String SPEECH_URL = "https://api.replicastudios.com/speech";
    String VOICES_URL = "https://api.replicastudios.com/v2/voices";
    String Access_Token = "";
    String API_AUTH_URL = "https://api.replicastudios.com/v2/auth";
    String clientId = "client ID";
    String clientPassword = "client Password";
    String speakerId = "934f1043-402c-4167-ba18-9a7ee0f1a178";

    //"37b607b0-aa6a-4421-8026-480c0138a61d";
    //b29d1a44-263b-4e83-803f-b6ce4bcbca25 // slime
    //c4fe46c4-79c0-403e-9318-ffe7bd4247dd // belzebub
    //990eeb48-3c4a-4cd1-9a55-f66d09bf0116 //Oak
    //211710e3-9ecb-437c-a6e1-893d78936b32 // noise
    //934f1043-402c-4167-ba18-9a7ee0f1a178 // pink Dragon
    //0be14b8f-fc12-48f1-8be7-cb52845789ad // pink Dragon 2
    //202e864f-4107-44f0-a7f1-de6f58f689f3 // pink Dragon jovial

    SyncProcessQueueObject<byte[]> replicaStudioQueue;
    public ReplicaStudioAPI(SyncProcessQueueObject<byte[]> replicaStudioQueue) {
        this.replicaStudioQueue = replicaStudioQueue;
    }

    public HttpRequest makeRequest(String content) throws UnsupportedEncodingException {
        String modelChain = "classic";
        System.out.println("Access Token: " + Access_Token);
        String encodedContent = URLEncoder.encode(content, StandardCharsets.UTF_8);
        String url = String.format("%s?txt=%s&speaker_id=%s&model_chain=%s", SPEECH_URL, encodedContent, speakerId, modelChain);
        System.out.println("Url: " + url);
        return HttpRequest.newBuilder()
                .uri(URI.create(url))
                .header("Authorization", "Bearer " + Access_Token)
                .GET()
                .build();
    }

    public void sendRequest(HttpRequest request) {
        HttpClient client = HttpClient.newHttpClient();

        try {
            HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());

            if (response.statusCode() == 200) {
                String jsonResponse = response.body();

                String audioFileUrl = jsonResponse.split("\"url\":\"")[1].split("\"")[0];
                String fixedUrl = fixUrl(audioFileUrl);
                replicaStudioQueue.putQueue(urlToByteArray2(fixedUrl));
            } else {
                System.out.println("GET request failed. Response Code: " + response.statusCode());
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    public void setAccess_Token() {
        try {
            String params = "client_id=" + URLEncoder.encode(clientId, StandardCharsets.UTF_8)
                    + "&secret=" + URLEncoder.encode(clientPassword, StandardCharsets.UTF_8);
            HttpRequest request = HttpRequest.newBuilder()
                    .uri(URI.create(API_AUTH_URL))
                    .header("Content-Type", "application/x-www-form-urlencoded")
                    .POST(HttpRequest.BodyPublishers.ofString(params))
                    .build();

            HttpClient client = HttpClient.newHttpClient();
            HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());

            if (response.statusCode() == 200) {
                JsonObject jsonResponse = gson.fromJson(response.body(), JsonObject.class);
                this.Access_Token = jsonResponse.get("access_token").getAsString();
            } else {
                System.out.println("Authentication failed. Response Code: " + response.statusCode());
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    public static String fixUrl(String rawUrl) {
        String unescapedUrl = rawUrl.replace("\\/", "/");

        String fixedUrl = unescapedUrl.replaceAll("^(https?:)/+", "$1//");

        return fixedUrl;
    }

}
