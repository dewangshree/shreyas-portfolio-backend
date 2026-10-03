pipeline {
    agent any

    environment {
        DOTNET = '/usr/local/share/dotnet/dotnet'
        PATH = "/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin:${env.PATH}"
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

        stage('Sonar Begin') {
            steps {
                script {
                    def scannerHome = tool 'SonarScanner for .NET'

                    withSonarQubeEnv('SonarQube') {
                        sh """
                            "\$DOTNET" "${scannerHome}/SonarScanner.MSBuild.dll" begin \
                              /k:"dewangshree_shreyas-portfolio-backend_c5739113-917d-4ff1-90e1-afe846b95866" \
                              /d:sonar.token="\$SONAR_AUTH_TOKEN"
                        """
                    }
                }
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

        stage('Sonar End') {
            steps {
                script {
                    def scannerHome = tool 'SonarScanner for .NET'

                    withSonarQubeEnv('SonarQube') {
                        sh """
                            "\$DOTNET" "${scannerHome}/SonarScanner.MSBuild.dll" end \
                              /d:sonar.token="\$SONAR_AUTH_TOKEN"
                        """
                    }
                }
            }
        }

        stage('Quality Gate') {
            steps {
                timeout(time: 5, unit: 'MINUTES') {
                    waitForQualityGate abortPipeline: true
                }
            }
        }

        stage('Docker Build') {
            steps {
                sh '''
                    /usr/local/bin/docker build \
                      -t shreyas-portfolio-backend:${BUILD_NUMBER} \
                      -t shreyas-portfolio-backend:latest \
                      .
                '''
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
                    set -e

                    echo "Stopping Shreyas Portfolio API..."
                    ssh -i ~/.ssh/shree shree@54.37.159.71 \
                      'sudo systemctl stop shreyas-portfolio-api'

                    echo "Syncing new backend files..."
                    rsync -az \
                      -e "ssh -i ~/.ssh/shree" \
                      publish-linux/ \
                      shree@54.37.159.71:/var/www/shreyas-portfolio/backend/

                    echo "Starting Shreyas Portfolio API..."
                    ssh -i ~/.ssh/shree shree@54.37.159.71 \
                      'sudo systemctl start shreyas-portfolio-api'

                    echo "Waiting for API..."
                    sleep 3

                    echo "Checking health..."
                    ssh -i ~/.ssh/shree shree@54.37.159.71 \
                      'curl --fail --silent --show-error http://127.0.0.1:5105/health'
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
            echo 'Backend CI/CD completed successfully.'
        }

        failure {
            echo 'Backend CI/CD failed.'
        }
    }
}