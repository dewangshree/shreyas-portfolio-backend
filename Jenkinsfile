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
                    rm -rf publish

                    "$DOTNET" publish src/Shreyas.Profile.Api/Shreyas.Profile.Api.csproj \
                      --configuration Release \
                      --output publish \
                      --no-build
                '''
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts artifacts: 'publish/**', fingerprint: true
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