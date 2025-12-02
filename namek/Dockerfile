FROM node:20.19.0

ARG BUILD_STAGE
ARG environment="alpha"
ARG namekVersion="latest"
ARG dockerport=3000
ARG API_GATEWAY_URL=""

ENV PROJECT_VERSION $namekVersion
ENV BUILD_STAGE $BUILD_STAGE
ENV ENVIRONMENT $environment
ENV DOCKER_PORT $dockerport
ENV API_GATEWAY_URL $API_GATEWAY_URL

RUN mkdir /namek
WORKDIR /namek
COPY ./package.json /namek

RUN apt-get update \
 && apt-get install -y python3 python3-pip unzip curl jq \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

# Install AWS CLI v2 (optional - comment out if not needed)
# RUN curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip" \
#  && unzip awscliv2.zip \
#  && ./aws/install \
#  && rm -rf awscliv2.zip aws

RUN npm install --verbose

COPY . /namek
RUN sed -i "s#latest#$projectVersion#" ./index.html

EXPOSE $dockerport
RUN npm run build

# Start the application
CMD ["npm", "run", "server"]
