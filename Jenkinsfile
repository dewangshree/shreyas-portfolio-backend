pipeline {
    agent any

    environment {
        DOTNET = '/usr/local/share/dotnet/dotnet'
    }

    options {
        disableConcurrentBuilds()
        timestamps()
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Verify .NET') {
            steps {
                sh '"$DOTNET" --version'
            }
        }

        stage('Restore') {
            steps {
                sh '"$DOTNET" restore'
            }
        }

        stage('Build') {
            steps {
                sh '"$DOTNET" build --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh '"$DOTNET" test --configuration Release --no-build'
            }
        }

        stage('Publish') {
            steps {
                sh '''
                    rm -rf publish-linux

                    "$DOTNET" publish src/Shreyas.Profile.Api/Shreyas.Profile.Api.csproj \
                      --configuration Release \
                      --runtime linux-x64 \
                      --self-contained true \
                      --output publish-linux
                '''
            }
        }

        stage('Deploy') {
            steps {
                sh '''
                    scp -i ~/.ssh/shree -r publish-linux/. \
                      shree@54.37.159.71:/var/www/shreyas-portfolio/backend/

                    ssh -i ~/.ssh/shree shree@54.37.159.71 \
                      'sudo systemctl restart shreyas-portfolio-api && curl --fail --silent --show-error http://127.0.0.1:5105/health'
                '''
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts artifacts: 'publish-linux/**', fingerprint: true
            }
        }
    }

    post {
        success {
            echo 'Backend CI completed successfully.'
        }

        failure {
            echo 'Backend CI failed.'
        }
    }
}
