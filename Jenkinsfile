pipeline {
    agent any

    environment {
        PATH = "/usr/local/share/dotnet:/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin:${env.PATH}"
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

        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test --configuration Release --no-build'
            }
        }

        stage('Publish') {
            steps {
                sh '''
                    rm -rf publish

                    dotnet publish src/Shreyas.Profile.Api/Shreyas.Profile.Api.csproj \
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