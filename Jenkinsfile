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
                              /d:sonar.token="\$SONAR_AUTH_TOKEN" \
                              /d:sonar.scanner.scanAll=false \
                              /d:sonar.exclusions="**/bin/**,**/obj/**,**/publish/**,**/publish-linux/**"
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

        stage('Docker Build & Deploy') {
            steps {
                sh '''
                    set -e

                    echo "Syncing backend source code to server..."

                    rsync -az --delete \
                      --exclude '.git' \
                      --exclude 'bin' \
                      --exclude 'obj' \
                      --exclude 'publish' \
                      --exclude 'publish-linux' \
                      --exclude '.scannerwork' \
                      -e "ssh -i ~/.ssh/shree" \
                      ./ \
                      shree@54.37.159.71:/home/shree/shreyas-portfolio-backend-build/

                    echo "Building Docker image on server..."

                    ssh -i ~/.ssh/shree shree@54.37.159.71 "
                        set -e

                        cd /home/shree/shreyas-portfolio-backend-build

                        sudo docker build \
                          -t shreyas-portfolio-backend:${BUILD_NUMBER} \
                          -t shreyas-portfolio-backend:latest \
                          .

                        echo 'Replacing backend container...'

                        sudo docker rm -f shreyas-portfolio-backend 2>/dev/null || true

                        sudo docker run -d \
                          --name shreyas-portfolio-backend \
                          --restart unless-stopped \
                          -p 127.0.0.1:5106:10000 \
                          shreyas-portfolio-backend:${BUILD_NUMBER}
                    "

                    echo "Waiting for backend container..."
                    sleep 3

                    echo "Checking backend health..."

                    ssh -i ~/.ssh/shree shree@54.37.159.71 \
                      'curl --fail --silent --show-error http://127.0.0.1:5106/health'

                    echo "Checking public API..."

                    curl --fail --silent --show-error \
                      https://shreyasportfolio.hopto.org/api/quotes/today > /dev/null

                    echo "Backend Docker build and deployment successful."
                '''
            }
        }
    }

    post {
        success {
            echo 'Backend CI/CD + SonarQube + server-side Docker deployment completed successfully.'
        }

        failure {
            echo 'Backend CI/CD failed.'
        }
    }
}