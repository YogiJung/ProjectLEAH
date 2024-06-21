package Utils;

import com.google.gson.JsonObject;

public class ResponseFormat {
    String data;
    int backPressureFlag;
    JsonObject header = new JsonObject();
    //sendingSignal : PAUSE, START
    public ResponseFormat(String endpoint) {
        header.addProperty("endpoint", endpoint);
        backPressureFlag = BackPressure.backPressureFlag;
    }

    public void setData(String data) {
        this.data = data;
    }

    public String getData() {
        return data;
    }
}
