echo "BUILD_STAGE: ${BUILD_STAGE}"
python3 -u rogue.py './src/config/env/template-config.json' "${PROJECT_CONFIG}" "${BUILD_STAGE}"

# แสดงค่า Configuration ที่ถูกสร้างขึ้น
CONFIG_FILE="./src/config/env/${BUILD_STAGE}-config.json"
echo ""
echo "--- Configuration Values ---"
echo "    Config File: ${CONFIG_FILE}"

if [ -f "${CONFIG_FILE}" ]; then
    # แสดงค่าทุกตัวแปรใน config file อัตโนมัติ
    cat "${CONFIG_FILE}" | jq -r 'to_entries | .[] | "    \(.key): \(.value)"'
else
    echo "    Config file not found: ${CONFIG_FILE}"
fi

echo ""

ENV_CONFIG=`cat "./src/config/env/${BUILD_STAGE}-config.json"`
export ENV_CONFIG
npm run server