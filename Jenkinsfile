pipeline {
    agent any

    environment {
        DOTNET = '/usr/local/share/dotnet/dotnet'
        PATH = "/Applications/Docker.app/Contents/Resources/bin:/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin:${env.PATH}"
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
                    docker build --load \
                      -t shreyas-portfolio-backend:${BUILD_NUMBER} \
                      -t shreyas-portfolio-backend:latest \
                      .

                    docker image inspect \
                      shreyas-portfolio-backend:${BUILD_NUMBER} > /dev/null
                '''
            }
        }

        stage('Docker Deploy') {
            steps {
                sh '''
                    set -e

                    echo "Sending backend Docker image to server..."

                    docker save shreyas-portfolio-backend:${BUILD_NUMBER} | gzip | \
                    ssh -i ~/.ssh/shree shree@54.37.159.71 \
                      'gunzip | sudo docker load'

                    echo "Replacing backend container..."

                    ssh -i ~/.ssh/shree shree@54.37.159.71 "
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

                    echo "Backend Docker deployment successful."
                '''
            }
        }
    }

    post {
        success {
            echo 'Backend CI/CD + SonarQube + Docker deployment completed successfully.'
        }

        failure {
            echo 'Backend CI/CD failed.'
        }
    }
}